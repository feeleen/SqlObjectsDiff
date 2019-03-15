using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerDiff
{
	public partial class WaitForm : Form
	{
		public Action Worker { get; set; }
		public CancellationToken CancellationToken { get; set; }
		private CancellationTokenSource cancelSource = new CancellationTokenSource();

		public WaitForm()
		{
			InitializeComponent();
		}

		public void SetStatus(string status)
		{
			try
			{
				if (InvokeRequired)
				{
					Invoke((MethodInvoker)delegate
					{
						SetStatus(status);
					});
					return;
				}

				if (this == null)
					return;

				StatusLabel.Text = status;
			}
			catch { }
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
			try
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

				LogBox.AppendText(message + Environment.NewLine);
				LogBox.ScrollToCaret();
			}
			catch { }
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

			CancellationToken = cancelSource.Token;

			Task.Factory.StartNew(Worker).ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void WaitForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			cancelSource.Cancel();
		}
	}
}
