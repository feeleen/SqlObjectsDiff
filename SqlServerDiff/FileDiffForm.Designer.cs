namespace Feeleen.Diff
{
	partial class FileDiffForm
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
			this.DiffCtrl = new Menees.Diffs.Controls.DiffControl();
			this.SuspendLayout();
			// 
			// DiffCtrl
			// 
			this.DiffCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DiffCtrl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.DiffCtrl.LineDiffHeight = 47;
			this.DiffCtrl.Location = new System.Drawing.Point(0, 0);
			this.DiffCtrl.Name = "DiffCtrl";
			this.DiffCtrl.OverviewWidth = 38;
			this.DiffCtrl.ShowWhiteSpaceInLineDiff = true;
			this.DiffCtrl.Size = new System.Drawing.Size(920, 536);
			this.DiffCtrl.TabIndex = 0;
			this.DiffCtrl.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DiffCtrl.LineDiffSizeChanged += new System.EventHandler(this.DiffCtrl_LineDiffSizeChanged);
			this.DiffCtrl.RecompareNeeded += new System.EventHandler(this.DiffCtrl_RecompareNeeded);
			this.DiffCtrl.ShowTextDifferences += new System.EventHandler<Menees.Diffs.Controls.DifferenceEventArgs>(this.DiffCtrl_ShowTextDifferences);
			// 
			// FileDiffForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(920, 536);
			this.Controls.Add(this.DiffCtrl);
			this.Name = "FileDiffForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Diff: Main Server - vs - Test Server";
			this.Closed += new System.EventHandler(this.FileDiffForm_Closed);
			this.Shown += new System.EventHandler(this.FileDiffForm_Shown);
			this.ResumeLayout(false);

		}
		#endregion
	}
}