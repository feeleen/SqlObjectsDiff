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

namespace SqlServerDiff
{
    public partial class Form1 : Form
    {

        ServerConnection mainConn = new ServerConnection("ds");
        ServerConnection testConn = new ServerConnection("TEMP-sql1");

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
            get { return MainServer.Databases["drugman"]; }
        }

        public Database TestDB
        {
            get { return TestServer.Databases["drugman1"]; }
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
                where modify_date > dateadd(m, -3, getdate())
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

            //List down all the user-defined function of AdventureWorks
            //foreach (UserDefinedFunction myUserDefinedFunction in MainDB.UserDefinedFunctions)
            //{
             //   Console.WriteLine(myUserDefinedFunction.Name);
            //}
        }

        private void AnalyzeDiffBtn_Click(object sender, EventArgs e)
        {
            GetObjects(MainDB);
            GetObjects(TestDB);
        }

        private void GetObjects(Database db)
        {
            foreach (Table tbl in db.Tables)
            {
                if (!ChangedObjectsTest.ContainsKey(tbl.Name))
                    continue;

                if (db == MainDB)
                {
                    TableTextMain[tbl.Name] = GetTableText(db, tbl.Name);
                }
                else
                {
                    TableTextTest[tbl.Name] = GetTableText(db, tbl.Name);
                }

                if (!NodeTables.Nodes.ContainsKey(tbl.Name))
                {
                    TreeNode nt = new TreeNode(tbl.Name);
                    nt.Name = tbl.Name;
                    NodeTables.Nodes.Add(nt);
                }

                foreach (Trigger tr in tbl.Triggers)
                {
                    if (!ChangedObjectsTest.ContainsKey(tr.Name) || ChangedObjectsTest[tr.Name] != "TR")
                        continue;

                    if (db == MainDB)
                    {
                        triggersToTablesMain[tr.Name] = tbl.Name;
                    }
                    else
                    {
                        triggersToTablesTest[tr.Name] = tbl.Name;
                    }
                }
            }


            foreach (Microsoft.SqlServer.Management.Smo.View v in db.Views)
            {
                if (!ChangedObjectsTest.ContainsKey(v.Name) || ChangedObjectsTest[v.Name] != "V")
                    continue;

                if (db == MainDB)
                {
                    ViewTextMain[v.Name] = GetViewText(db, v.Name);
                }
                else
                {
                    ViewTextTest[v.Name] = GetViewText(db, v.Name);
                }
            }


            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                if (!ChangedObjectsTest.ContainsKey(sp.Name) || ChangedObjectsTest[sp.Name] != "P")
                    continue;

                if (db == MainDB)
                {
                    SPTextMain[sp.Name] = GetStoredProcedureText(db, sp.Name);
                }
                else
                {
                    SPTextTest[sp.Name] = GetStoredProcedureText(db, sp.Name);
                }
            }

            foreach (UserDefinedFunction uf in db.UserDefinedFunctions)
            {
                if (!ChangedObjectsTest.ContainsKey(uf.Name))// || ChangedObjectsTest[uf.Name] != )
                    continue;

                if (db == MainDB)
                {
                    UFTextMain[uf.Name] = GetUFText(db, uf.Name);
                }
                else
                {
                    UFTextTest[uf.Name] = GetUFText(db, uf.Name);
                }
            }

            foreach (UserDefinedType ut in db.UserDefinedTypes)
            {
                if (!ChangedObjectsTest.ContainsKey(ut.Name) || ChangedObjectsTest[ut.Name] != "TT")
                    continue;


                if (db == MainDB)
                {
                    UFTextMain[ut.Name] = GetUFText(db, ut.Name);
                }
                else
                {
                    UFTextTest[ut.Name] = GetUFText(db, ut.Name);
                }
            }
        }
    }
}
