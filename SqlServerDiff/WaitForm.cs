using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerDiff
{
	public partial class WaitForm : Form
	{
		public Action Worker { get; set; }

		public WaitForm()
		{
			InitializeComponent();
		}

		public void SetStatus(string status)
		{
			if (InvokeRequired)
			{
				Invoke((MethodInvoker)delegate {
					SetStatus(status);
				});
				return;
			}

			StatusLabel.Text = status;
			
		}

		public string Status
		{
			get
			{
				return StatusLabel.Text;
			}
			set
			{
				SetStatus(value);
			}
		}

		public void AddLog(string message)
		{
			if (InvokeRequired)
			{
				Invoke((MethodInvoker)delegate {
					AddLog(message);
				});
				return;
			}

			if (LogBox.TextLength > 30000)
				LogBox.Text = "";

			LogBox.AppendText(message+ Environment.NewLine);
			LogBox.ScrollToCaret();
		}

		public string Log
		{
			get
			{
				return LogBox.Text;
			}
			set
			{
				AddLog(value);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Task.Factory.StartNew(Worker).ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
