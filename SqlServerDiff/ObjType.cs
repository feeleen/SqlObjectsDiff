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
		public const string AggregateFunction = "AF";

		/*
		public const string TableValuedFunction = "TF";
		public const string ExtendedStoredProcedure = "X";
		public const string SqlInlineTableValuedFunction = "IF";
		public const string PrimaryKeyConstraint = "PK";

		public const string CheckConstraint = "C";
		public const string DefaultConstraint = "D";
		public const string ForeignKeyConstraint = "F";
		public const string ClrScalarFunction = "FS";
		public const string ClrStoredProcedure = "PC";
		*/

		public static string GetDescription(string objType)
		{
			return typeof(ObjType)
			  .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
			  .Where(f => f.FieldType == typeof(string) && (string)f.GetValue(null) == objType)
			  .Select(f => (string)f.Name).FirstOrDefault();
		}

		public static List<string> GetAllObjTypes()
		{
			return typeof(ObjType)
			  .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
			  .Where(f => f.FieldType == typeof(string))
			  .Select(f => (string)f.GetValue(null)).ToList();
		}
	}
}
