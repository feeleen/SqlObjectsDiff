using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Feeleen.Diff
{
	public enum DiffType
	{
		File,
		Directory,
		Text
	};

	public enum DialogDisplay
	{
		Always,
		UseOption,
		OnlyIfNecessary
	};

	public interface IDifferenceForm
	{
		void ShowDifferences(ShowDiffArgs e);
		void UpdateUI();
	}

	public interface IDifferenceDlg
	{
		string NameA
		{
			get;
			set;
		}
		string NameB
		{
			get;
			set;
		}
		bool RequiresInput
		{
			get;
		}
		bool OnlyShowIfShiftPressed
		{
			get;
		}
		bool ShowShiftCheck
		{
			set;
		}
		DialogResult ShowDialog(IWin32Window Owner);
	}
}
