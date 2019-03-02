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

		Dictionary<string, Dictionary<string, string>> ChangedObjectsTest = null;

		Dictionary<string, Dictionary<string, string>> AllObjectsTest = null;
		Dictionary<string, Dictionary<string, string>> AllObjectsMain = null;


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
            if (MainSchema.SPText.ContainsKey(textBox1.Text.Trim()) || TestSchema.SPText.ContainsKey(textBox1.Text.Trim()))
            {
                ShowDifferences(MainSchema.GetStoredProcedureText(textBox1.Text.Trim()), TestSchema.GetStoredProcedureText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.TriggersToTables.ContainsKey(textBox1.Text.Trim()) || TestSchema.TriggersToTables.ContainsKey(textBox1.Text.Trim()))
            {
                ShowDifferences(MainSchema.GetTriggerText(textBox1.Text.Trim()), TestSchema.GetTriggerText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.ViewText.ContainsKey(textBox1.Text.Trim()) || TestSchema.ViewText.ContainsKey(textBox1.Text.Trim()))
            { 
                ShowDifferences(MainSchema.GetViewText(textBox1.Text.Trim()), TestSchema.GetViewText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.TableText.ContainsKey(textBox1.Text.Trim()) || TestSchema.TableText.ContainsKey(textBox1.Text.Trim()))
            {
                ShowDifferences(MainSchema.GetTableText(textBox1.Text.Trim()), TestSchema.GetTableText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.UFText.ContainsKey(textBox1.Text.Trim()) || TestSchema.UFText.ContainsKey(textBox1.Text.Trim()))
            {
                ShowDifferences(MainSchema.GetUFText(textBox1.Text.Trim()), TestSchema.GetUFText(textBox1.Text.Trim()), DiffType.Text);
            }
            else if (MainSchema.UTText.ContainsKey(textBox1.Text.Trim()) || TestSchema.UTText.ContainsKey(textBox1.Text.Trim()))
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

			using (WaitForm wf = new WaitForm(LoadObjects))
			{
				wf.ShowDialog(this);
			}
        }

		public void LoadObjects()
		{
			if (RawCompareBox.Checked)
			{
				AllObjectsTest = TestSchema.GetAllObjects();
				AllObjectsMain = MainSchema.GetAllObjects();

				// union all objects from main & test
				Dictionary<string, Dictionary<string, string>> differentObjects = new Dictionary<string, Dictionary<string, string>>();

				foreach (string oType in AllObjectsMain.Keys)
				{
					if (!differentObjects.ContainsKey(oType))
						differentObjects[oType] = new Dictionary<string, string>();

					foreach (string mainObjName in AllObjectsMain[oType].Keys)
					{
						if (!differentObjects[oType].ContainsKey(oType))
							differentObjects[oType][mainObjName] = oType;
					}
				}

				foreach (string oType in AllObjectsTest.Keys)
				{
					if (!differentObjects.ContainsKey(oType))
						differentObjects[oType] = new Dictionary<string, string>();

					foreach (string mainObjName in AllObjectsTest[oType].Keys)
					{
						if (!differentObjects[oType].ContainsKey(oType))
							differentObjects[oType][mainObjName] = oType;
					}
				}

				GetObjects(MainSchema, TestSchema, differentObjects);
			}
			else
			{
				// track changes only from test!
				ChangedObjectsTest = TestSchema.GetChangedObjects(Convert.ToInt32(DaysBox.Value));
				GetObjects(MainSchema, TestSchema, ChangedObjectsTest);
			}
		}

		public void AppendChildNode(TreeNode parent, string tblName, string tag)
		{
			if (InvokeRequired)
			{
				Invoke((MethodInvoker)delegate {
					AppendChildNode(parent, tblName, tag);
				});
				return;
			}

			TreeNode nt = new TreeNode(tblName);
			nt.Name = tblName;
			nt.Tag = tag;
			parent.Nodes.Add(nt);
			parent.Text = $"{parent.ToolTipText} ({parent.Nodes.Count})";
		}

        private void GetObjects(SchemaContainer schema, SchemaContainer schemaTest, Dictionary<string, Dictionary<string, string>> changedObjects)
        {
			if (changedObjects.ContainsKey(ObjType.Table))
			{
				foreach (string tblName in changedObjects[ObjType.Table].Select(s => s.Key).ToList())
				{
					if (!changedObjects[ObjType.Table].ContainsKey(tblName))
						continue;

					schema.TableText[tblName] = schema.GetTableText(tblName);
					schemaTest.TableText[tblName] = schemaTest.GetTableText(tblName);

					if (schemaTest.TableText[tblName] != schema.TableText[tblName])
					{
						AppendChildNode(NodeTables, tblName, ObjType.Table);
					}

					if (changedObjects.ContainsKey(ObjType.TableTrigger))
					{
						//foreach (Trigger tr in schema.Database.Tables[tblName].Triggers)
						foreach (string trName in changedObjects[ObjType.TableTrigger].Select(s => s.Key).ToList())
						{
							if (!changedObjects.ContainsKey(trName))
								continue;

							//schema.TriggersToTables[trName] = tblName;
							schema.TriggerText[trName] = schema.GetTriggerText(trName);

							schemaTest.TriggersToTables[trName] = tblName;
							schemaTest.TriggerText[trName] = schemaTest.GetTriggerText(trName);

							if (schemaTest.TriggerText[trName] != schema.TriggerText[trName])
							{
								AppendChildNode(NodeTriggers, trName, ObjType.TableTrigger);
							}
						}
					}
				}
			}

			if (changedObjects.ContainsKey(ObjType.View))
			{
				foreach (string vName in changedObjects[ObjType.View].Select(s => s.Key).ToList())
				{
					if (!changedObjects[ObjType.View].ContainsKey(vName))
						continue;

					schema.ViewText[vName] = schema.GetViewText(vName);
					schemaTest.ViewText[vName] = schemaTest.GetViewText(vName);

					if (schemaTest.ViewText[vName]!= schema.ViewText[vName])
					{
						AppendChildNode(NodeViews, vName, ObjType.View);
					}
				}
			}

			if (changedObjects.ContainsKey(ObjType.StoredProcedure))
			{
				foreach (string spName in changedObjects[ObjType.StoredProcedure].Select(s => s.Key).ToList())
				{
					if (!changedObjects[ObjType.StoredProcedure].ContainsKey(spName))
						continue;

					schema.SPText[spName] = schema.GetStoredProcedureText(spName);
					schemaTest.SPText[spName] = schemaTest.GetStoredProcedureText(spName);

					if (schemaTest.SPText[spName]!= schema.SPText[spName])
					{
						AppendChildNode(NodeSP, spName, ObjType.StoredProcedure);
					}
				}
			}

			if (changedObjects.ContainsKey(ObjType.UserFunction))
			{
				foreach (string ufName in changedObjects[ObjType.UserFunction].Select(s => s.Key).ToList())
				{
					if (!changedObjects[ObjType.UserFunction].ContainsKey(ufName))
						continue;

					schema.UFText[ufName] = schema.GetUFText(ufName);
					schemaTest.UFText[ufName] = schemaTest.GetUFText(ufName);

					if (schemaTest.UFText[ufName]!= schema.UFText[ufName])
					{
						AppendChildNode(NodeUF, ufName, ObjType.UserFunction);
					}
				}
			}

			if (changedObjects.ContainsKey(ObjType.UserTableType))
			{
				foreach (string utName in changedObjects[ObjType.UserTableType].Select(s => s.Key).ToList())
				{
					if (!changedObjects[ObjType.UserTableType].ContainsKey(utName))
						continue;

					schema.UTText[utName] = schema.GetUTTableText(utName);
					schemaTest.UTText[utName] = schemaTest.GetUTTableText(utName);

					if (schemaTest.UTText[utName]!= schema.UTText[utName])
					{
						AppendChildNode(NodeUT, utName, ObjType.UserTableType);
					}
				}
			}
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            fctb.Text = GetNodeScript(e.Node);
            textBox1.Text = e.Node.Name;
        }

        private string GetNodeScript(TreeNode node)
        {
            if (node.Tag == null)
                return String.Empty;

            switch (node.Tag.ToString())
            {
                case ObjType.UserTableType:
                    return TestSchema.UTText[node.Name] == null ? MainSchema.UTText[node.Name] : TestSchema.UTText[node.Name];

                case ObjType.Table:
                    return TestSchema.TableText[node.Name] == null ? MainSchema.TableText[node.Name] : TestSchema.TableText[node.Name];

				case ObjType.TableTrigger:
                    return TestSchema.TriggerText[node.Name] == null ? MainSchema.TriggerText[node.Name] : TestSchema.TriggerText[node.Name];

				case ObjType.UserFunction:
                    return TestSchema.UFText[node.Name] == null ? MainSchema.UFText[node.Name] : TestSchema.UFText[node.Name];

				case ObjType.StoredProcedure:
                    return TestSchema.SPText[node.Name] == null ? MainSchema.SPText[node.Name] : TestSchema.SPText[node.Name];

				case ObjType.View:
                    return TestSchema.ViewText[node.Name] == null ? MainSchema.ViewText[node.Name] : TestSchema.ViewText[node.Name];

				default:
                    break;
            }

            return String.Empty;
        }
		
    }
}
