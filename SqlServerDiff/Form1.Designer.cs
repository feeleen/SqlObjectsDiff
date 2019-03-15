namespace SqlServerDiff
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Tables");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Triggers");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Stored Procedures");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Functions");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("User Types");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Views");
			this.ViewDiffBtn = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SearchBtn = new System.Windows.Forms.Button();
			this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.AnalyzeDiffBtn = new System.Windows.Forms.Button();
			this.DaysBox = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.RawCompareBox = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.fctb)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DaysBox)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ViewDiffBtn
			// 
			this.ViewDiffBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ViewDiffBtn.Location = new System.Drawing.Point(928, 12);
			this.ViewDiffBtn.Name = "ViewDiffBtn";
			this.ViewDiffBtn.Size = new System.Drawing.Size(99, 23);
			this.ViewDiffBtn.TabIndex = 0;
			this.ViewDiffBtn.Text = "View Diff";
			this.ViewDiffBtn.UseVisualStyleBackColor = true;
			this.ViewDiffBtn.Click += new System.EventHandler(this.ViewDiffBtn_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(92, 14);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(362, 20);
			this.textBox1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Object search:";
			// 
			// SearchBtn
			// 
			this.SearchBtn.Location = new System.Drawing.Point(457, 12);
			this.SearchBtn.Name = "SearchBtn";
			this.SearchBtn.Size = new System.Drawing.Size(75, 23);
			this.SearchBtn.TabIndex = 3;
			this.SearchBtn.Text = "Search";
			this.toolTip1.SetToolTip(this.SearchBtn, "searchMain server for object");
			this.SearchBtn.UseVisualStyleBackColor = true;
			this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
			// 
			// fctb
			// 
			this.fctb.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.fctb.AutoIndentCharsPatterns = "";
			this.fctb.AutoIndentExistingLines = false;
			this.fctb.AutoScrollMinSize = new System.Drawing.Size(32, 15);
			this.fctb.BackBrush = null;
			this.fctb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fctb.CharHeight = 15;
			this.fctb.CharWidth = 7;
			this.fctb.CommentPrefix = "--";
			this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.fctb.DelayedEventsInterval = 200;
			this.fctb.DelayedTextChangedInterval = 500;
			this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fctb.Font = new System.Drawing.Font("Consolas", 9.75F);
			this.fctb.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.fctb.IsReplaceMode = false;
			this.fctb.Language = FastColoredTextBoxNS.Language.SQL;
			this.fctb.LeftBracket = '(';
			this.fctb.Location = new System.Drawing.Point(0, 0);
			this.fctb.Name = "fctb";
			this.fctb.Paddings = new System.Windows.Forms.Padding(0);
			this.fctb.PreferredLineWidth = 80;
			this.fctb.ReservedCountOfLineNumberChars = 2;
			this.fctb.RightBracket = ')';
			this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.fctb.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctb.ServiceColors")));
			this.fctb.Size = new System.Drawing.Size(679, 384);
			this.fctb.TabIndex = 5;
			this.fctb.Zoom = 100;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			treeNode1.Name = "NodeTables";
			treeNode1.Text = "Tables";
			treeNode2.Name = "NodeTriggers";
			treeNode2.Text = "Triggers";
			treeNode3.Name = "NodeSP";
			treeNode3.Text = "Stored Procedures";
			treeNode4.Name = "NodeFn";
			treeNode4.Text = "Functions";
			treeNode5.Name = "NodeUserTypes";
			treeNode5.Text = "User Types";
			treeNode6.Name = "NodeViews";
			treeNode6.Text = "Views";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
			this.treeView1.Size = new System.Drawing.Size(340, 384);
			this.treeView1.TabIndex = 6;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDoubleClick);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(4, 41);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.fctb);
			this.splitContainer1.Size = new System.Drawing.Size(1023, 384);
			this.splitContainer1.SplitterDistance = 340;
			this.splitContainer1.TabIndex = 7;
			// 
			// AnalyzeDiffBtn
			// 
			this.AnalyzeDiffBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AnalyzeDiffBtn.Location = new System.Drawing.Point(773, 12);
			this.AnalyzeDiffBtn.Name = "AnalyzeDiffBtn";
			this.AnalyzeDiffBtn.Size = new System.Drawing.Size(149, 23);
			this.AnalyzeDiffBtn.TabIndex = 8;
			this.AnalyzeDiffBtn.Text = "Analyze Changed Objects";
			this.AnalyzeDiffBtn.UseVisualStyleBackColor = true;
			this.AnalyzeDiffBtn.Click += new System.EventHandler(this.AnalyzeDiffBtn_Click);
			// 
			// DaysBox
			// 
			this.DaysBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.DaysBox.Location = new System.Drawing.Point(714, 14);
			this.DaysBox.Name = "DaysBox";
			this.DaysBox.Size = new System.Drawing.Size(53, 20);
			this.DaysBox.TabIndex = 9;
			this.DaysBox.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(677, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Days:";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 428);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1030, 22);
			this.statusStrip1.TabIndex = 11;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(1015, 17);
			this.StatusLabel.Spring = true;
			this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// RawCompareBox
			// 
			this.RawCompareBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RawCompareBox.AutoSize = true;
			this.RawCompareBox.Location = new System.Drawing.Point(583, 15);
			this.RawCompareBox.Name = "RawCompareBox";
			this.RawCompareBox.Size = new System.Drawing.Size(92, 17);
			this.RawCompareBox.TabIndex = 12;
			this.RawCompareBox.Text = "Raw compare";
			this.toolTip1.SetToolTip(this.RawCompareBox, "Raw comparison of all objects of Main and Test servers (it may take a long time f" +
        "or big databases)");
			this.RawCompareBox.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1030, 450);
			this.Controls.Add(this.RawCompareBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.DaysBox);
			this.Controls.Add(this.AnalyzeDiffBtn);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.SearchBtn);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.ViewDiffBtn);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "SqlServerDiff - Compare objects between Main and Test Sql Servers";
			((System.ComponentModel.ISupportInitialize)(this.fctb)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.DaysBox)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ViewDiffBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SearchBtn;
        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button AnalyzeDiffBtn;
        private System.Windows.Forms.NumericUpDown DaysBox;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		private System.Windows.Forms.CheckBox RawCompareBox;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}

