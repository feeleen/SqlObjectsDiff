using System;
using System.Collections.Generic;
using System.Text;

namespace Feeleen.Diff
{
	public class ShowDiffArgs
	{
		#region Constructors

		public ShowDiffArgs(string strA, string strB, DiffType eType)
		{
			m_strA = strA;
			m_strB = strB;
			m_eType = eType;
		}

		#endregion

		#region Public Properties

		public string A
		{
			get
			{
				return m_strA;
			}
		}
		public string B
		{
			get
			{
				return m_strB;
			}
		}
		public DiffType DiffType
		{
			get
			{
				return m_eType;
			}
		}

		#endregion

		#region Private Data Members

		private string m_strA;
		private string m_strB;
		private DiffType m_eType;

		#endregion
	}
}