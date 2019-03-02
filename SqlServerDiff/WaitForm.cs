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

		public WaitForm(Action worker)
		{
			InitializeComponent();

			Worker = worker;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Task.Factory.StartNew(Worker).ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
