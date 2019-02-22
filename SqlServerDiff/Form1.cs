using Menees.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Feeleen.Diff;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SqlServerDiff
{
    public partial class Form1 : Form
    {

        ServerConnection mainConn = new ServerConnection(ConfigurationManager.AppSettings["MainServer"]);
        ServerConnection testConn = new ServerConnection(ConfigurationManager.AppSettings["TestServer"]);

        Dictionary<string, string> triggersToTablesMain = new Dictionary<string, string>();
        Dictionary<string, string> triggersToTablesTest = new Dictionary<string, string>();

        Dictionary<string, string> SPTextMain = new Dictionary<string, string>();
        Dictionary<string, string> SPTextTest = new Dictionary<string, string>();

        Dictionary<string, string> TriggerTextMain = new Dictionary<string, string>();
        Dictionary<string, string> TriggerTextTest = new Dictionary<string, string>();

        Dictionary<string, string> ViewTextMain = new Dictionary<string, string>();
        Dictionary<string, string> ViewTextTest = new Dictionary<string, string>();

        Dictionary<string, string> UFTextMain = new Dictionary<string, string>();
        Dictionary<string, string> UFTextTest = new Dictionary<string, string>();

        Dictionary<string, string> UTTextMain = new Dictionary<string, string>();
        Dictionary<string, string> UTTextTest = new Dictionary<string, string>();

        Dictionary<string, string> TableTextMain = new Dictionary<string, string>();
        Dictionary<string, string> TableTextTest = new Dictionary<string, string>();

        Dictionary<string, string> ChangedObjectsMain = new Dictionary<string, string>();
        Dictionary<string, string> ChangedObjectsTest = new Dictionary<string, string>();

        public Server MainServer
        {
            get
            {

                Server myServer = new Server(mainConn);
                return myServer;
            }
        }

        public Database MainDB
        {
            get {
                string mainDBName = ConfigurationManager.AppSettings["MainDatabase"].Replace(" ", "");

                if (!Regex.IsMatch(mainDBName, @"^[a-zA-Z0-9_]+$"))
                    throw new Exception("Main database name is not correct!");

                return MainServer.Databases[mainDBName];
            }
        }

        public Database TestDB
        {
            get {
                string testDBName = ConfigurationManager.AppSettings["TestDatabase"].Replace(" ", "");

                if (!Regex.IsMatch(testDBName, @"^[a-zA-Z0-9_]+$"))
                    throw new Exception("Test database name is not correct!");

                return TestServer.Databases[testDBName];
            }
        }

        public Server TestServer
        {
            get
            {
                Server lsrv = new Server(testConn);
                return lsrv;
            }
        }

        TreeNode NodeTables = new TreeNode("Tables");
        TreeNode NodeViews = new TreeNode("Views");
        TreeNode NodeSP = new TreeNode("Stored Procedures");
        TreeNode NodeUT = new TreeNode("User Types");
        TreeNode NodeUF = new TreeNode("User Functions");
        TreeNode NodeTriggers = new TreeNode("Triggers");

        public Form1()
		{
			InitializeComponent();

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(NodeTables);
            treeView1.Nodes.Add(NodeViews);
            treeView1.Nodes.Add(NodeSP);
            treeView1.Nodes.Add(NodeUT);
            treeView1.Nodes.Add(NodeUF);
            treeView1.Nodes.Add(NodeTriggers);

            mainConn.LoginSecure = false;
            testConn.LoginSecure = false;
            
            PasswordForm frm = new PasswordForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                mainConn.Login = frm.LoginBox.Text;
                mainConn.Password = frm.PasswordBox.Text;
                testConn.Login = frm.LoginBox.Text;
                testConn.Password = frm.testPasswordBox.Text;
            }
            else
                Application.Exit();

            mainConn.Connect();
            testConn.Connect();

            ChangedObjectsTest = GetChangedObjects(TestDB);


            foreach (Table tbl in MainDB.Tables)
            {
                foreach (Trigger tr in tbl.Triggers)
                {
                    triggersToTablesMain[tr.Name] = tbl.Name;
                }
            }

		}

        private Dictionary<string, string> GetChangedObjects(Database db)
        {
            string sql = $@"use {db.Name};
                select
                    name,
                    type,
                    type_desc
                from sys.all_objects
                where modify_date > dateadd(d, -{DaysBox.Value}, getdate())
                order by modify_date desc";

            DataSet set = db.ExecuteWithResults(sql);

            Dictionary<string, string> objects = new Dictionary<string, string>();

            foreach (DataTable t in set.Tables)
            {
                foreach (DataRow row in t.Rows)
                    objects[row["name"].ToString()] = row["type"].ToString();
                break;
            }

            return objects;
        }


        private void TextArea_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Top-level method for showing differences.")]
		private void ShowDifferences(string itemA, string itemB, DiffType diffType)
		{
			using (WaitCursor wc = new WaitCursor(this))
			{	
				Form frmNew = new FileDiffForm();
				frmNew.Owner  = this;
                frmNew.Text = "Diff: Main Server - vs - Test Server";

                IDifferenceForm frmDiff = (IDifferenceForm)frmNew;
                frmDiff.ShowDifferences(new ShowDiffArgs(itemA == null ? "" : itemA, itemB == null ? "" : itemB, diffType));
			}
		}

        public string GetViewText(Database db, string name)
        {
            if (db.Views[name] == null)
                return null;

            return db.Views[name].TextHeader + Environment.NewLine + db.Views[name].TextBody;
        }

        public string GetStoredProcedureText(Database db, string name)
        {
            if (db.StoredProcedures[name] == null)
                return null;

            return db.StoredProcedures[name].TextHeader + Environment.NewLine + db.StoredProcedures[name].TextBody;
        }

        public string GetUTText(Database db, string name)
        {
            if (db.UserDefinedTypes[name] == null)
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (string s in db.UserDefinedTypes[name].Script())
            {
                builder.AppendLine(s);
            }
            string result = builder.ToString();

            return result;
        }

        public string GetUFText(Database db, string name)
        {
            if (db.UserDefinedFunctions[name] == null)
                return null;

            return db.UserDefinedFunctions[name].TextHeader + Environment.NewLine + db.UserDefinedFunctions[name].TextBody;
        }

        public string GetTableText(Database db, string name)
        {
            if (db.Tables[name] == null)
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (string s in db.Tables[name].Script())
            {
                builder.AppendLine(s);
            }
            string result = builder.ToString();

            return result;
        }


        public string GetUTTableText(Database db, string name)
        {
            if (db.UserDefinedTableTypes[name] == null)
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (string s in db.UserDefinedTableTypes[name].Script())
            {
                builder.AppendLine(s);
            }
            string result = builder.ToString();

            return result;
        }

        public string GetTriggerText(Database db, string name)
        {
            string tableName = "";

            if (triggersToTablesMain.ContainsKey(name) && triggersToTablesMain[name] != null)
            {
                tableName = triggersToTablesMain[name];
            }
            else
                return null;

            foreach (Trigger tr in db.Tables[tableName].Triggers)
            {
                if (tr.Name == name)
                {
                    return tr.TextHeader + Environment.NewLine + tr.TextBody;
                }
            }

            return null;
        }

        private void button1_Click(object sender, EventArgs e)
		{
            if (MainDB.StoredProcedures[textBox1.Text.Trim()] != null || TestDB.StoredProcedures[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(GetStoredProcedureText(MainDB, textBox1.Text.Trim()), GetStoredProcedureText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
            else if (triggersToTablesMain.ContainsKey(textBox1.Text.Trim()) && triggersToTablesMain[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(GetTriggerText(MainDB, textBox1.Text.Trim()), GetTriggerText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainDB.Views[textBox1.Text.Trim()] != null)
            { 
                ShowDifferences(GetViewText(MainDB, textBox1.Text.Trim()), GetViewText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainDB.Tables[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(GetTableText(MainDB, textBox1.Text.Trim()), GetTableText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainDB.UserDefinedFunctions[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(GetUFText(MainDB, textBox1.Text.Trim()), GetUFText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainDB.UserDefinedTableTypes[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(GetUTTableText(MainDB, textBox1.Text.Trim()), GetUTTableText(TestDB, textBox1.Text.Trim()), DiffType.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (GetStoredProcedureText(MainDB, textBox1.Text.Trim()) != null)
            {
                fctb.Text = GetStoredProcedureText(MainDB, textBox1.Text.Trim());
            }
            else if (GetTriggerText(MainDB, textBox1.Text.Trim()) != null)
            {
                fctb.Text = GetTriggerText(MainDB, textBox1.Text.Trim());
            }
            else if (GetViewText(MainDB, textBox1.Text.Trim()) != null)
            {
                fctb.Text = GetViewText(MainDB, textBox1.Text.Trim());
            }
            else if (GetTableText(MainDB, textBox1.Text.Trim()) != null)
            {
                fctb.Text = GetTableText(MainDB, textBox1.Text.Trim());
            }
            else if (GetUTTableText(MainDB, textBox1.Text.Trim()) != null)
            {
                fctb.Text = GetUTTableText(MainDB, textBox1.Text.Trim());
            }

            //List down all the user-defined function of AdventureWorks
            //foreach (UserDefinedFunction myUserDefinedFunction in MainDB.UserDefinedFunctions)
            //{
            //   Console.WriteLine(myUserDefinedFunction.Name);
            //}
        }

        private void AnalyzeDiffBtn_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            GetObjects(MainDB);
            GetObjects(TestDB);
        }

        private void GetObjects(Database db)
        {
            foreach (string tblName in ChangedObjectsTest.Where(s => s.Value.Trim() == "U").Select(s => s.Key).ToList())
            {
                if (db == MainDB)
                {
                    TableTextMain[tblName] = GetTableText(db, tblName);
                }
                else
                {
                    TableTextTest[tblName] = GetTableText(db, tblName);
                }

                if (!NodeTables.Nodes.ContainsKey(tblName))
                {
                    TreeNode nt = new TreeNode(tblName);
                    nt.Name = tblName;
                    nt.Tag = "U";
                    NodeTables.Nodes.Add(nt);
                }

                foreach (Trigger tr in db.Tables[tblName].Triggers)
                {
                    if (!ChangedObjectsTest.ContainsKey(tr.Name))
                        continue;

                    if (db == MainDB)
                    {
                        triggersToTablesMain[tr.Name] = tblName;
                        TriggerTextMain[tr.Name] = GetTriggerText(db, tr.Name);
                    }
                    else
                    {
                        triggersToTablesTest[tr.Name] = tblName;
                        TriggerTextTest[tr.Name] = GetTriggerText(db, tr.Name);
                    }

                    if (!NodeTriggers.Nodes.ContainsKey(tr.Name))
                    {
                        TreeNode nt = new TreeNode(tr.Name);
                        nt.Name = tr.Name;
                        nt.Tag = "TR";
                        NodeTriggers.Nodes.Add(nt);
                    }
                }
            }

            foreach (string vName in ChangedObjectsTest.Where(s => s.Value.Trim() == "V").Select(s => s.Key).ToList())
            {
            //foreach (Microsoft.SqlServer.Management.Smo.View v in db.Views)
            //{
                if (!ChangedObjectsTest.ContainsKey(vName))
                    continue;

                if (db == MainDB)
                {
                    ViewTextMain[vName] = GetViewText(db, vName);
                }
                else
                {
                    ViewTextTest[vName] = GetViewText(db, vName);
                }

                if (!NodeViews.Nodes.ContainsKey(vName))
                {
                    TreeNode nt = new TreeNode(vName);
                    nt.Name = vName;
                    nt.Tag = "V";
                    NodeViews.Nodes.Add(nt);
                }
            }


            foreach (string spName in ChangedObjectsTest.Where(s => s.Value.Trim() == "P").Select(s => s.Key).ToList())
            {
            //foreach (StoredProcedure sp in db.StoredProcedures)
            //{
                if (!ChangedObjectsTest.ContainsKey(spName))
                    continue;

                if (db == MainDB)
                {
                    SPTextMain[spName] = GetStoredProcedureText(db, spName);
                }
                else
                {
                    SPTextTest[spName] = GetStoredProcedureText(db, spName);
                }

                if (!NodeSP.Nodes.ContainsKey(spName))
                {
                    TreeNode nt = new TreeNode(spName);
                    nt.Name = spName;
                    nt.Tag = "P";
                    NodeSP.Nodes.Add(nt);
                }
            }

            foreach (string ufName in ChangedObjectsTest.Where(s => s.Value.Trim() == "FN").Select(s => s.Key).ToList())
            {
                //foreach (UserDefinedFunction uf in db.UserDefinedFunctions)
                //{
                if (!ChangedObjectsTest.ContainsKey(ufName))
                    continue;

                if (db == MainDB)
                {
                    UFTextMain[ufName] = GetUFText(db, ufName);
                }
                else
                {
                    UFTextTest[ufName] = GetUFText(db, ufName);
                }

                if (!NodeUF.Nodes.ContainsKey(ufName))
                {
                    TreeNode nt = new TreeNode(ufName);
                    nt.Name = ufName;
                    nt.Tag = "FN";
                    NodeUF.Nodes.Add(nt);
                }
            }

            foreach (string utName in ChangedObjectsTest.Where(s => s.Value.Trim() == "TT").Select(s => s.Key).ToList())
            {
                //foreach (UserDefinedType ut in db.UserDefinedTypes)
                //{
                if (!ChangedObjectsTest.ContainsKey(utName))
                    continue;

                if (db == MainDB)
                {
                    UTTextMain[utName] = GetUTTableText(db, utName);
                }
                else
                {
                    UTTextTest[utName] = GetUTTableText(db, utName);
                }

                if (!NodeUT.Nodes.ContainsKey(utName))
                {
                    TreeNode nt = new TreeNode(utName);
                    nt.Name = utName;
                    nt.Tag = "TT";
                    NodeUT.Nodes.Add(nt);
                }
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            fctb.Text = GetNodeTestScript(e.Node);
            textBox1.Text = e.Node.Name;
        }

        private string GetNodeMainScript(TreeNode node)
        {
            if (node.Tag == null)
                return String.Empty;

            switch (node.Tag.ToString())
            {
                case "TT":
                    return UTTextMain[node.Name];

                case "U":
                    return TableTextMain[node.Name];

                case "TR":
                    return TriggerTextMain[node.Name];

                case "FN":
                    return UFTextMain[node.Name];

                case "P":
                    return SPTextMain[node.Name];

                case "V":
                    return ViewTextMain[node.Name];

                default:
                    break;
            }

            return String.Empty;
        }

        private string GetNodeTestScript(TreeNode node)
        {
            if (node.Tag == null)
                return String.Empty;

            switch (node.Tag.ToString())
            {
                case "TT":
                    return UTTextTest[node.Name];

                case "U":
                    return TableTextTest[node.Name];

                case "TR":
                    return TriggerTextTest[node.Name];

                case "FN":
                    return UFTextTest[node.Name];

                case "P":
                    return SPTextTest[node.Name];

                case "V":
                    return ViewTextTest[node.Name];

                default:
                    break;
            }

            return String.Empty;
        }
    }
}
