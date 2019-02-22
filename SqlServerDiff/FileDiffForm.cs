using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml;
using Menees;
using Menees.Diffs;
using System.IO;
using System.Text;
using Menees.Windows.Forms;
using Menees.Diffs.Controls;
using System.Collections.Generic;
using Diff.Net;

namespace Feeleen.Diff
{
	/// <summary>
	/// Summary description for FileDiffForm.
	/// </summary>
	public partial class FileDiffForm : Form, IDifferenceForm
	{
		private Menees.Diffs.Controls.DiffControl DiffCtrl;
		private EventHandler m_OptionsChangedHandler;
		private ShowDiffArgs m_CurrentDiffArgs;

		public FileDiffForm()
		{
			InitializeComponent();
			//System.Windows.Forms.VisualStyles.EnableControls(this);

			m_OptionsChangedHandler = new EventHandler(OptionsChanged);
			Options.OptionsChanged += m_OptionsChangedHandler;
		}

		

		private void mnuCopy_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.Copy();
		}

		private void mnuFind_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.Find();
		}

		private void mnuFindNext_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.FindNext();
		}

		private void mnuFindPrevious_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.FindPrevious();
		}

		private void mnuGoToNextDiff_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.GoToNextDiff();
		}

		private void mnuGoToPreviousDiff_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.GoToPreviousDiff();
		}

		private void mnuGoToLine_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.GoToLine();
		}

		public void ShowDifferences(ShowDiffArgs e)
		{
			string strA = e.A;
			string strB = e.B;
			DiffType eType = e.DiffType;

			IList<string> A, B;
			int iLeadingCharactersToIgnore = 0;
			bool bFileNames = eType == DiffType.File;
			if (bFileNames)
			{
				GetFileLines(strA, strB, out A, out B, out iLeadingCharactersToIgnore);
			}
			else
			{
				GetTextLines(strA, strB, out A, out B);
			}

			TextDiff Diff = new TextDiff(Options.HashType, Options.IgnoreCase, true, iLeadingCharactersToIgnore, true);
			EditScript Script = Diff.Execute(A, B);

			string strCaptionA = "";
			string strCaptionB = "";

			if (bFileNames)
			{
				strCaptionA = strA;
				strCaptionB = strB;
				string fnA = FileUtility.ExpandFileName(strA);
				string fnB = FileUtility.ExpandFileName(strB);
				Text = String.Format("{0} : {1}", fnA , fnB );
			}
			
			DiffCtrl.SetData(A, B, Script, strCaptionA, strCaptionB);

			if (Options.LineDiffHeight != 0)
			{
				DiffCtrl.LineDiffHeight = Options.LineDiffHeight;
			}

			ApplyOptions();

			Show();

			m_CurrentDiffArgs = e;
		}

		public void UpdateUI()
		{
		}

		private void OptionsChanged(object sender, EventArgs e)
		{
			ApplyOptions();
		}

		private void ApplyOptions()
		{
			DiffCtrl.ShowToolBar = true;
			DiffCtrl.ShowColorLegend = true;
			DiffCtrl.UseTranslucentOverview = true;
			DiffCtrl.ShowWhiteSpaceInLineDiff = true;
			DiffCtrl.ViewFont = Options.ViewFont;
		}

		private void DiffCtrl_LineDiffSizeChanged(object sender, System.EventArgs e)
		{
			if (Visible)
			{
				Options.LineDiffHeight = DiffCtrl.LineDiffHeight;
			}
		}

		private void FileDiffForm_Closed(object sender, System.EventArgs e)
		{
			Options.OptionsChanged -= m_OptionsChangedHandler;
		}

		private void mnuViewFile_Click(object sender, System.EventArgs e)
		{
			DiffCtrl.ViewFile();
		}

		private void FileDiffForm_Shown(object sender, System.EventArgs e)
		{
			GoToFirstDiff();
		}

		private void GoToFirstDiff()
		{
			if (Options.GoToFirstDiff)
			{
				DiffCtrl.GoToFirstDiff();
			}
		}

		private static void GetFileLines(string fileNameA, string fileNameB, out IList<string> a, out IList<string> b, out int leadingCharactersToIgnore)
		{
			a = null;
			b = null;
			leadingCharactersToIgnore = 0;
			CompareType compareType = Options.CompareType;
			bool isAuto = compareType == CompareType.Auto;

			if (compareType == CompareType.Binary ||
				(isAuto && (DiffUtility.IsBinaryFile(fileNameA) || DiffUtility.IsBinaryFile(fileNameB))))
			{
				using (FileStream fileA = File.OpenRead(fileNameA))
				using (FileStream fileB = File.OpenRead(fileNameB))
				{
					BinaryDiff diff = new BinaryDiff();
					diff.FootprintLength = Options.BinaryFootprintLength;
					AddCopyCollection addCopy = diff.Execute(fileA, fileB);

					BinaryDiffLines lines = new BinaryDiffLines(fileA, addCopy, Options.BinaryFootprintLength);
					a = lines.BaseLines;
					b = lines.VersionLines;
					leadingCharactersToIgnore = BinaryDiffLines.PrefixLength;
				}
			}

			if (compareType == CompareType.Xml || (isAuto && (a == null || b == null)))
			{
				a = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameA, fileNameA, !isAuto);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (a != null)
				{
					b = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameB, fileNameB, !isAuto);
				}

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (a == null || b == null)
			{
				a = DiffUtility.GetFileTextLines(fileNameA);
				b = DiffUtility.GetFileTextLines(fileNameB);
			}
		}

		private static void GetTextLines(string textA, string textB, out IList<string> a, out IList<string> b)
		{
			a = null;
			b = null;
			CompareType compareType = Options.CompareType;
			bool isAuto = compareType == CompareType.Auto;

			if (compareType == CompareType.Xml || isAuto)
			{
				a = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the left side text", textA, !isAuto);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (a != null)
				{
					b = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the right side text", textB, !isAuto);
				}

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (a == null || b == null)
			{
				a = DiffUtility.GetStringTextLines(textA);
				b = DiffUtility.GetStringTextLines(textB);
			}
		}

		private static IList<string> TryGetXmlLines(
			Func<string, bool, IList<string>> converter,
			string name,
			string input,
			bool throwOnError)
		{
			IList<string> result = null;
			try
			{
				result = converter(input, Options.IgnoreXmlWhitespace);
			}
			catch (XmlException ex)
			{
				if (throwOnError)
				{
					StringBuilder sb = new StringBuilder("An XML comparison was attempted, but an XML exception occurred while parsing ");
					sb.Append(name).AppendLine(".").AppendLine();
					sb.AppendLine("Exception Message:").Append(ex.Message);
					throw new XmlException(sb.ToString(), ex);
				}
			}

			return result;
		}

		static void ShowDifferences(string strA, string strB, DiffType eType)
		{
			IDifferenceForm frmDiff = (IDifferenceForm)new FileDiffForm();
			frmDiff.ShowDifferences(new ShowDiffArgs(strA, strB, eType));
		}

		public static void ShowTextDifferences(string strA, string strB)
		{
			Options.LastTextA = strA;
			Options.LastTextB = strB;

			ShowDifferences(strA, strB, DiffType.Text);
		}


		private void DiffCtrl_ShowTextDifferences(object sender,  DifferenceEventArgs e)
		{
			FileDiffForm.ShowTextDifferences(e.ItemA, e.ItemB);
		}

		private void mnuCompareSelectedText_Click(object sender, EventArgs e)
		{
			DiffCtrl.CompareSelectedText();
		}

		private void mnuRecompare_Click(object sender, EventArgs e)
		{
			DiffCtrl.Recompare();
		}

		private void DiffCtrl_RecompareNeeded(object sender, EventArgs e)
		{
			if (m_CurrentDiffArgs != null)
			{
				using (WaitCursor WC = new WaitCursor(this))
				{
					ShowDifferences(m_CurrentDiffArgs);
					GoToFirstDiff();
				}
			}
		}

		private void mnuGoToFirstDiff_Click(object sender, EventArgs e)
		{
			DiffCtrl.GoToFirstDiff();
		}

		private void mnuGoToLastDiff_Click(object sender, EventArgs e)
		{
			DiffCtrl.GoToLastDiff();
		}
	}
}
