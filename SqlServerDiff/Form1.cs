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

        SchemaContainer MainSchema = new SchemaContainer(ServerType.Main);
		SchemaContainer TestSchema = new SchemaContainer(ServerType.Test);
		

        TreeNode NodeTables = new TreeNode("Tables");
		TreeNode NodeViews = new TreeNode("Views");
        TreeNode NodeSP = new TreeNode("Stored Procedures");
        TreeNode NodeUT = new TreeNode("User Types");
        TreeNode NodeUF = new TreeNode("User Functions");
        TreeNode NodeTriggers = new TreeNode("Triggers");

		Dictionary<string, string> ChangedObjects = null;


		public Form1()
		{
			InitializeComponent();
			CenterToScreen();

			NodeTables.ToolTipText = NodeTables.Text;
			NodeViews.ToolTipText = NodeViews.Text;
			NodeSP.ToolTipText = NodeSP.Text;
			NodeUT.ToolTipText = NodeUT.Text;
			NodeUF.ToolTipText = NodeUF.Text;
			NodeTriggers.ToolTipText = NodeTriggers.Text;

			treeView1.Nodes.Clear();
            treeView1.Nodes.Add(NodeTables);
            treeView1.Nodes.Add(NodeViews);
            treeView1.Nodes.Add(NodeSP);
            treeView1.Nodes.Add(NodeUT);
            treeView1.Nodes.Add(NodeUF);
            treeView1.Nodes.Add(NodeTriggers);

			using (PasswordForm frm = new PasswordForm())
			{
				frm.StartPosition = FormStartPosition.CenterParent;

				if (frm.ShowDialog() == DialogResult.OK)
				{
					if (!frm.MainISBox.Checked)
					{
						MainSchema.Connection.Login = frm.LoginBox.Text;
						MainSchema.Connection.Password = frm.PasswordBox.Text;
					}
					
					if (!frm.TestISBox.Checked)
					{
						TestSchema.Connection.Login = frm.TestLoginBox.Text;
						TestSchema.Connection.Password = frm.testPasswordBox.Text;
					}
					
				}
				else
					Application.Exit();
			}
			MainSchema.Connection.Connect();
			TestSchema.Connection.Connect();

			// track changes only from test!
			ChangedObjects = TestSchema.GetChangedObjects(Convert.ToInt32(DaysBox.Value));

			MainSchema.InitTriggerList();
			TestSchema.InitTriggerList();

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
		

        private void ViewDiffBtn_Click(object sender, EventArgs e)
		{
            if (MainSchema.Database.StoredProcedures[textBox1.Text.Trim()] != null || TestSchema.Database.StoredProcedures[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(MainSchema.GetStoredProcedureText(textBox1.Text.Trim()), TestSchema.GetStoredProcedureText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.TriggersToTables.ContainsKey(textBox1.Text.Trim()) || TestSchema.TriggersToTables.ContainsKey(textBox1.Text.Trim()))
            {
                ShowDifferences(MainSchema.GetTriggerText(textBox1.Text.Trim()), TestSchema.GetTriggerText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.Views[textBox1.Text.Trim()] != null || TestSchema.Views[textBox1.Text.Trim()] != null)
            { 
                ShowDifferences(MainSchema.GetViewText(textBox1.Text.Trim()), TestSchema.GetViewText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.Tables[textBox1.Text.Trim()] != null || TestSchema.Tables[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(MainSchema.GetTableText(textBox1.Text.Trim()), TestSchema.GetTableText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.UserFunctions[textBox1.Text.Trim()] != null || TestSchema.UserFunctions[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(MainSchema.GetUFText(textBox1.Text.Trim()), TestSchema.GetUFText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.UserTableTypes[textBox1.Text.Trim()] != null || TestSchema.UserTableTypes[textBox1.Text.Trim()] != null)
            {
                ShowDifferences(MainSchema.GetUTTableText(textBox1.Text.Trim()), TestSchema.GetUTTableText(textBox1.Text.Trim()), DiffType.Text);
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (MainSchema.GetStoredProcedureText(textBox1.Text.Trim()) != null)
            {
                fctb.Text = MainSchema.GetStoredProcedureText(textBox1.Text.Trim());
            }
            else if (MainSchema.GetTriggerText(textBox1.Text.Trim()) != null)
            {
                fctb.Text = MainSchema.GetTriggerText(textBox1.Text.Trim());
            }
            else if (MainSchema.GetViewText(textBox1.Text.Trim()) != null)
            {
                fctb.Text = MainSchema.GetViewText(textBox1.Text.Trim());
            }
            else if (MainSchema.GetTableText(textBox1.Text.Trim()) != null)
            {
                fctb.Text = MainSchema.GetTableText(textBox1.Text.Trim());
            }
            else if (MainSchema.GetUTTableText(textBox1.Text.Trim()) != null)
            {
                fctb.Text = MainSchema.GetUTTableText(textBox1.Text.Trim());
            }
        }

        private void AnalyzeDiffBtn_Click(object sender, EventArgs e)
        {
            NodeTables.Nodes.Clear();
            NodeViews.Nodes.Clear();
            NodeSP.Nodes.Clear();
            NodeUT.Nodes.Clear();
            NodeUF.Nodes.Clear();
            NodeTriggers.Nodes.Clear();

            GetObjects(MainSchema);
            GetObjects(TestSchema);
        }

		public void AppendChildNode(TreeNode parent, string tblName, string tag)
		{
			TreeNode nt = new TreeNode(tblName);
			nt.Name = tblName;
			nt.Tag = tag;
			parent.Nodes.Add(nt);
			parent.Text = $"{parent.ToolTipText} ({parent.Nodes.Count})";
		}

        private void GetObjects(SchemaContainer schema)
        {
			foreach (string tblName in ChangedObjects.Where(s => s.Value.Trim() == ObjType.Table).Select(s => s.Key).ToList())
            {
                schema.TableText[tblName] = schema.GetTableText(tblName);

				if (!NodeTables.Nodes.ContainsKey(tblName))
                {
					AppendChildNode(NodeTables, tblName, ObjType.Table);
				}

                foreach (Trigger tr in schema.Database.Tables[tblName].Triggers)
                {
                    if (!ChangedObjects.ContainsKey(tr.Name))
                        continue;

                    schema.TriggersToTables[tr.Name] = tblName;
					schema.TriggerText[tr.Name] = schema.GetTriggerText(tr.Name);
                    
                    if (!NodeTriggers.Nodes.ContainsKey(tr.Name))
                    {
						AppendChildNode(NodeTriggers, tr.Name, ObjType.TableTrigger);
					}
				}
            }

            foreach (string vName in ChangedObjects.Where(s => s.Value.Trim() == ObjType.View).Select(s => s.Key).ToList())
            {
            //foreach (Microsoft.SqlServer.Management.Smo.View v in db.Views)
            //{
                if (!ChangedObjects.ContainsKey(vName))
                    continue;

                schema.ViewText[vName] = schema.GetViewText(vName);
                

                if (!NodeViews.Nodes.ContainsKey(vName))
                {
					AppendChildNode(NodeViews, vName, ObjType.View);
                }
            }


            foreach (string spName in ChangedObjects.Where(s => s.Value.Trim() == ObjType.StoredProcedure).Select(s => s.Key).ToList())
            {
            //foreach (StoredProcedure sp in db.StoredProcedures)
            //{
                if (!ChangedObjects.ContainsKey(spName))
                    continue;

                schema.SPText[spName] = schema.GetStoredProcedureText(spName);

                if (!NodeSP.Nodes.ContainsKey(spName))
                {
					AppendChildNode(NodeSP, spName, ObjType.StoredProcedure);
                }
            }

            foreach (string ufName in ChangedObjects.Where(s => s.Value.Trim() == ObjType.UserFunction).Select(s => s.Key).ToList())
            {
                //foreach (UserDefinedFunction uf in db.UserDefinedFunctions)
                //{
                if (!ChangedObjects.ContainsKey(ufName))
                    continue;

                schema.UFText[ufName] = schema.GetUFText(ufName);

                if (!NodeUF.Nodes.ContainsKey(ufName))
                {
					AppendChildNode(NodeUF, ufName, ObjType.UserFunction);
                }
            }

            foreach (string utName in ChangedObjects.Where(s => s.Value.Trim() == ObjType.UserTableType).Select(s => s.Key).ToList())
            {
                //foreach (UserDefinedType ut in db.UserDefinedTypes)
                //{
                if (!ChangedObjects.ContainsKey(utName))
                    continue;

				schema.UTText[utName] = schema.GetUTTableText(utName);

                if (!NodeUT.Nodes.ContainsKey(utName))
                {
					AppendChildNode(NodeUT, utName, ObjType.UserTableType);
                }
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            fctb.Text = GetNodeScript(e.Node, TestSchema);
            textBox1.Text = e.Node.Name;
        }

        private string GetNodeScript(TreeNode node, SchemaContainer schema)
        {
            if (node.Tag == null)
                return String.Empty;

            switch (node.Tag.ToString())
            {
                case ObjType.UserTableType:
                    return schema.UTText[node.Name];

                case ObjType.Table:
                    return schema.TableText[node.Name];

                case ObjType.TableTrigger:
                    return schema.TriggerText[node.Name];

                case ObjType.UserFunction:
                    return schema.UFText[node.Name];

                case ObjType.StoredProcedure:
                    return schema.SPText[node.Name];

                case ObjType.View:
                    return schema.ViewText[node.Name];

                default:
                    break;
            }

            return String.Empty;
        }
		
    }
}
