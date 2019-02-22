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

namespace SqlServerDiff
{
    public partial class Form1 : Form
    {

        ServerConnection mainConn = new ServerConnection("MAINSERV");
        ServerConnection testConn = new ServerConnection("TEMPSERV");

        Dictionary<string, string> triggersToTables = new Dictionary<string, string>();

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
            get { return MainServer.Databases["mydb"]; }
        }

        public Database TestDB
        {
            get { return TestServer.Databases["mydb"]; }
        }

        public Server TestServer
        {
            get
            {
                Server lsrv = new Server(testConn);
                return lsrv;
            }
        }

		public Form1()
		{
			InitializeComponent();

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

            foreach (Database myDatabase in MainServer.Databases)
			{
				Console.WriteLine(myDatabase.Name);
			}
			
			foreach (Table myTable in MainDB.Tables)
			{
				Console.WriteLine(myTable.Name);
			}

			foreach (StoredProcedure myStoredProcedure in MainDB.StoredProcedures)
			{
				Console.WriteLine(myStoredProcedure.Name);
			}


            foreach (Table tbl in MainDB.Tables)
            {
                foreach (Trigger tr in tbl.Triggers)
                {
                    triggersToTables[tr.Name] = tbl.Name;
                }
            }

            //List down all the user-defined function of AdventureWorks
            foreach (UserDefinedFunction myUserDefinedFunction in MainDB.UserDefinedFunctions)
			{
				Console.WriteLine(myUserDefinedFunction.Name);
			}
			//List down all the properties and its values of [HumanResources].[Employee] table
			//foreach (Property myTableProperty in MainDB.Tables["tblInvoices", "dbo"].Properties)
			//{
			//	Console.WriteLine(myTableProperty.Name + " : " + myTableProperty.Value);
			//}
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

        public string GetTriggerText(Database db, string name)
        {
            string tableName = "";

            if (triggersToTables.ContainsKey(name) && triggersToTables[name] != null)
            {
                tableName = triggersToTables[name];
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
            else if (triggersToTables.ContainsKey(textBox1.Text.Trim()) && triggersToTables[textBox1.Text.Trim()] != null)
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
    }
}
