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
			NodeTables.Tag = ObjType.Table;

			NodeViews.ToolTipText = NodeViews.Text;
			NodeViews.Tag = ObjType.View;

			NodeSP.ToolTipText = NodeSP.Text;
			NodeSP.Tag = ObjType.StoredProcedure;

			NodeUT.ToolTipText = NodeUT.Text;
			NodeUT.Tag = ObjType.UserTableType;

			NodeUF.ToolTipText = NodeUF.Text;
			NodeUF.Tag = ObjType.UserFunction;

			NodeTriggers.ToolTipText = NodeTriggers.Text;
			NodeTriggers.Tag = ObjType.TableTrigger;

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
                    else
                        MainSchema.UseConnectionString();

                    if (!frm.TestISBox.Checked)
					{
						TestSchema.Connection.Login = frm.TestLoginBox.Text;
						TestSchema.Connection.Password = frm.testPasswordBox.Text;
					}
                    else
                        TestSchema.UseConnectionString();

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
				frmNew.StartPosition = FormStartPosition.CenterParent;

				IDifferenceForm frmDiff = (IDifferenceForm)frmNew;
                frmDiff.ShowDifferences(new ShowDiffArgs(itemA == null ? "" : itemA, itemB == null ? "" : itemB, diffType));
			}
		}
		

        private void ViewDiffBtn_Click(object sender, EventArgs e)
		{
			foreach (string objType in ObjType.GetAllObjTypes())
			{
				if (MainSchema.ObjectRepository[objType].ContainsKey(textBox1.Text.Trim()) || TestSchema.ObjectRepository[objType].ContainsKey(textBox1.Text.Trim()))
				{
					ShowDifferences(MainSchema.GetObjectSourceText(textBox1.Text.Trim(), objType), TestSchema.GetObjectSourceText(textBox1.Text.Trim(), objType), DiffType.Text);
				}
				else if (MainSchema.GetObjectSourceText(textBox1.Text.Trim(), objType) != null)
				{
					ShowDifferences(MainSchema.GetObjectSourceText(textBox1.Text.Trim(), objType), TestSchema.GetObjectSourceText(textBox1.Text.Trim(), objType), DiffType.Text);
				}
			}
		}

        private void SearchBtn_Click(object sender, EventArgs e)
        {
			foreach (string objType in ObjType.GetAllObjTypes())
			{
				string objSource = MainSchema.GetObjectSourceText(textBox1.Text.Trim(), objType);
				if (objSource != null)
				{
					fctb.Text = objSource;
				}
			}
        }

        private void AnalyzeDiffBtn_Click(object sender, EventArgs e)
        {
			foreach (TreeNode rootNode in treeView1.Nodes)
			{
				rootNode.Nodes.Clear();
			}

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

				LoadChangedObjects(MainSchema, TestSchema, differentObjects);
			}
			else
			{
				// track changes only from test!
				ChangedObjectsTest = TestSchema.GetChangedObjects(Convert.ToInt32(DaysBox.Value));
				LoadChangedObjects(MainSchema, TestSchema, ChangedObjectsTest);
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

        private void LoadChangedObjects(SchemaContainer schema, SchemaContainer schemaTest, Dictionary<string, Dictionary<string, string>> changedObjects)
        {
			foreach (string objType in ObjType.GetAllObjTypes())
			{
				LoadChangedObjectsByType(schema, schemaTest, changedObjects, objType);
			}
		}

		public List<string> LoadChangedObjectsByType(SchemaContainer schema, SchemaContainer schemaTest, Dictionary<string, Dictionary<string, string>> changedObjects, string objectType)
		{
			List<string> result = new List<string>();

			if (changedObjects.ContainsKey(objectType))
			{
				foreach (string objName in changedObjects[objectType].Select(s => s.Key).ToList())
				{
					if (!changedObjects[objectType].ContainsKey(objName))
						continue;

					schema.ObjectRepository[objectType][objName] = schema.GetObjectSourceText(objName, objectType);
					schemaTest.ObjectRepository[objectType][objName] = schemaTest.GetObjectSourceText(objName, objectType);

					if (schemaTest.ObjectRepository[objectType][objName] != schema.ObjectRepository[objectType][objName])
					{
						result.Add(objName);
						AppendChildNode(GetTreeNodeByType(objectType), objName, objectType);
					}

					// hack for triggers - we need to fill TriggersToTables additionally
					if (objectType == ObjType.Table)
					{
						List<string> triggers = LoadChangedObjectsByType(schema, schemaTest, changedObjects, ObjType.TableTrigger);

						foreach (string trName in triggers)
						{
							schemaTest.TriggersToTables[trName] = objName;
						}
					}
				}
			}
			return result;
		}

		private TreeNode GetTreeNodeByType(string nodeType)
		{
			foreach (TreeNode n in treeView1.Nodes)
			{
				if (n.Tag != null && n.Tag.ToString() == nodeType)
					return n;
			}

			return null;
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
            if (node.Tag == null || node.Parent == null)
                return String.Empty;

			string objType = node.Tag.ToString();

			return TestSchema.ObjectRepository[objType][node.Name] == null ? MainSchema.ObjectRepository[objType][node.Name] : TestSchema.ObjectRepository[objType][node.Name];
		}
		
    }
}
