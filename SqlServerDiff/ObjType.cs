using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerDiff
{
	public static class ObjType
	{
		public const string UserTableType = "TT";
		public const string Table = "U";
		public const string TableTrigger = "TR";
		public const string UserFunction = "FN";
		public const string StoredProcedure = "P";
		public const string View = "V";

		public static List<string> GetAllObjTypes()
		{
			return typeof(ObjType)
			  .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
			  .Where(f => f.FieldType == typeof(string))
			  .Select(f => (string)f.GetValue(null)).ToList();
		}
	}
}
