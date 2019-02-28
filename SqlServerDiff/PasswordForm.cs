using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerDiff
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();

			LoginBox.Text = ConfigurationManager.AppSettings["MainServerLogin"];
			TestLoginBox.Text = ConfigurationManager.AppSettings["TestServerLogin"];

		}

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void PasswordForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                DialogResult = DialogResult.OK;
        }

        private void testPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                DialogResult = DialogResult.OK;
        }

		private void MainISBox_CheckedChanged(object sender, EventArgs e)
		{
			PasswordBox.Enabled = !MainISBox.Checked;
			LoginBox.Enabled = !MainISBox.Checked;
		}

		private void TestISBox_CheckedChanged(object sender, EventArgs e)
		{
			testPasswordBox.Enabled = !TestISBox.Checked;
			TestLoginBox.Enabled = !TestISBox.Checked;
		}
	}
}
