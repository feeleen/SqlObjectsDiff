namespace SqlServerDiff
{
    partial class PasswordForm
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
			this.LoginBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.PasswordBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.testPasswordBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.MainISBox = new System.Windows.Forms.CheckBox();
			this.TestISBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.TestLoginBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// LoginBox
			// 
			this.LoginBox.Location = new System.Drawing.Point(69, 49);
			this.LoginBox.Name = "LoginBox";
			this.LoginBox.Size = new System.Drawing.Size(107, 20);
			this.LoginBox.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Login";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(333, 147);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(91, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "ОК";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// PasswordBox
			// 
			this.PasswordBox.Location = new System.Drawing.Point(69, 75);
			this.PasswordBox.Name = "PasswordBox";
			this.PasswordBox.PasswordChar = '*';
			this.PasswordBox.Size = new System.Drawing.Size(107, 20);
			this.PasswordBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Password";
			// 
			// testPasswordBox
			// 
			this.testPasswordBox.Location = new System.Drawing.Point(75, 75);
			this.testPasswordBox.Name = "testPasswordBox";
			this.testPasswordBox.PasswordChar = '*';
			this.testPasswordBox.Size = new System.Drawing.Size(107, 20);
			this.testPasswordBox.TabIndex = 3;
			this.testPasswordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.testPasswordBox_KeyDown);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17, 78);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Password";
			// 
			// MainISBox
			// 
			this.MainISBox.AutoSize = true;
			this.MainISBox.Location = new System.Drawing.Point(69, 19);
			this.MainISBox.Name = "MainISBox";
			this.MainISBox.Size = new System.Drawing.Size(115, 17);
			this.MainISBox.TabIndex = 5;
			this.MainISBox.Text = "Integrated Security";
			this.MainISBox.UseVisualStyleBackColor = true;
			this.MainISBox.CheckedChanged += new System.EventHandler(this.MainISBox_CheckedChanged);
			// 
			// TestISBox
			// 
			this.TestISBox.AutoSize = true;
			this.TestISBox.Location = new System.Drawing.Point(75, 19);
			this.TestISBox.Name = "TestISBox";
			this.TestISBox.Size = new System.Drawing.Size(115, 17);
			this.TestISBox.TabIndex = 5;
			this.TestISBox.Text = "Integrated Security";
			this.TestISBox.UseVisualStyleBackColor = true;
			this.TestISBox.CheckedChanged += new System.EventHandler(this.TestISBox_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.PasswordBox);
			this.groupBox1.Controls.Add(this.LoginBox);
			this.groupBox1.Controls.Add(this.MainISBox);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(195, 118);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Main server*";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.TestLoginBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.TestISBox);
			this.groupBox2.Controls.Add(this.testPasswordBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new System.Drawing.Point(226, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(198, 118);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Test server*";
			// 
			// TestLoginBox
			// 
			this.TestLoginBox.Location = new System.Drawing.Point(75, 49);
			this.TestLoginBox.Name = "TestLoginBox";
			this.TestLoginBox.Size = new System.Drawing.Size(107, 20);
			this.TestLoginBox.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(33, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Login";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(288, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "*Use App.config to specify server names && database names";
			// 
			// PasswordForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(436, 182);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.Name = "PasswordForm";
			this.Text = "Login";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasswordForm_KeyDown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox LoginBox;
        public System.Windows.Forms.TextBox PasswordBox;
        public System.Windows.Forms.TextBox testPasswordBox;
        private System.Windows.Forms.Label label5;
		public System.Windows.Forms.CheckBox MainISBox;
		public System.Windows.Forms.CheckBox TestISBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox TestLoginBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
	}
}