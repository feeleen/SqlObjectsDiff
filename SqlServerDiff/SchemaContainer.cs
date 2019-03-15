using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlServerDiff
{
	public class SchemaContainer
	{
		private ServerType servertype;
		private ServerConnection conn = new ServerConnection();
		private Server myServer = null;

		public Dictionary<string, string> TriggersToTables = new Dictionary<string, string>();

		public Dictionary<string, Dictionary<string, string>> ObjectRepository = new Dictionary<string, Dictionary<string, string>>();

		public SchemaContainer(ServerType servertype)
		{
			this.servertype = servertype;
			conn.ServerInstance = ServerName;
			conn.LoginSecure = false;
			myServer = new Server(conn);

			foreach (string objType in ObjType.GetAllObjTypes())
			{
				ObjectRepository[objType] = new Dictionary<string, string>();
			}
		}

		public Server Server
		{
			get
			{

				return myServer;
			}
		}

		public Database Database
		{
			get
			{
				if (!Regex.IsMatch(DBName, @"^[a-zA-Z0-9_]+$"))
					throw new Exception(ServerTypeName + " database name is not correct!");

				return this.Server.Databases[DBName];
			}
		}

		public string ServerTypeName
		{
			get
			{
				return Enum.GetName(typeof(ServerType), servertype);
			}
		}

		public ServerConnection Connection
		{
			get {
				return conn;
			}
			set { }
		}

        public void UseConnectionString()
        {
            conn.ConnectionString = GetSSPIConnectionString(ServerName, DBName);
        }

		public string ServerName
		{
			get { return ConfigurationManager.AppSettings[ServerTypeName + "Server"].Replace(" ", ""); }
		}
		
		public string DBName
		{
			get { return ConfigurationManager.AppSettings[ServerTypeName + "Database"].Replace(" ", ""); }
		}
		
		private string GetSSPIConnectionString(string server, string databaseName, string userName = "", string password = "")
		{
			var builder = new SqlConnectionStringBuilder
			{
				DataSource = server, // server address
				InitialCatalog = databaseName, // database name
				IntegratedSecurity = true, // server auth(false)/win auth(true)
				MultipleActiveResultSets = false, // activate/deactivate MARS
				PersistSecurityInfo = true, // hide login credentials
				//UserID = userName, // user name
				//Password = password // password
			};
			return builder.ConnectionString;
		}

		public void InitTriggerList()
		{
			foreach (Table tbl in Database.Tables)
			{
				foreach (Trigger tr in tbl.Triggers)
				{
					TriggersToTables[tr.Name] = tbl.Name;
				}
			}
		}

		public Dictionary<string, Dictionary<string, string>> GetAllObjects()
		{
			return GetChangedObjects(-1);
		}

		public Dictionary<string, Dictionary<string, string>> GetChangedObjects(int days)
		{
			string sql = $@"use {Database.Name};
                select
                    name,
                    type,
                    type_desc
                from sys.all_objects
                where modify_date > dateadd(d, -{days}, getdate())
                order by modify_date desc";

			if (days < 0)
			{
				sql = $@"use {Database.Name};
                select
                    name,
                    type,
                    type_desc
                from sys.all_objects
                order by modify_date desc";
			}

			DataSet set = Database.ExecuteWithResults(sql);

			List<string> distinctTypes = null;

			foreach (DataTable t in set.Tables)
			{
				distinctTypes = t.AsEnumerable()
					   .Select(s => new
					   {
						   id = s.Field<string>("type").ToString().ToUpper().Trim(),
					   })
					   .Distinct().ToList().Select(s=> s.id.ToString()).ToList();
				break;
			}

			Dictionary<string, Dictionary<string, string>> objects = new Dictionary<string, Dictionary<string, string>>();

			foreach (string ty in distinctTypes)
			{
				Dictionary<string, string> finalObjects = new Dictionary<string, string>();

				foreach (DataTable t in set.Tables)
				{
					
					foreach (DataRow row in t.Select("type = '" + ty + "'"))
					{
						finalObjects[row["name"].ToString()] = row["type"].ToString().Trim();
					}
					break;
				}
				objects[ty] = finalObjects;
			}

			return objects;
		}

		private string GetViewText(string name)
		{
			if (Database.Views[name] == null)
				return null;

			return Database.Views[name].TextHeader + Environment.NewLine + Database.Views[name].TextBody;
		}

		private string GetStoredProcedureText(string name)
		{
			if (Database.StoredProcedures[name] == null)
				return null;

			return Database.StoredProcedures[name].TextHeader + Environment.NewLine + Database.StoredProcedures[name].TextBody;
		}

		private string GetUTText(string name)
		{
			if (Database.UserDefinedTypes[name] == null)
				return null;

			StringBuilder builder = new StringBuilder();
			foreach (string s in Database.UserDefinedTypes[name].Script())
			{
				builder.AppendLine(s);
			}
			string result = builder.ToString();

			return result;
		}

		private string GetUFText(string name)
		{
			if (Database.UserDefinedFunctions[name] == null)
				return null;

			return Database.UserDefinedFunctions[name].TextHeader + Environment.NewLine + Database.UserDefinedFunctions[name].TextBody;
		}

		private string GetTableText(string name)
		{
			if (Database.Tables[name] == null)
				return null;

			ScriptingOptions so = new ScriptingOptions();
			so.DriAllConstraints = true;
			so.DriForeignKeys = true;
			so.DriIndexes = true;
			so.NoCollation = true;

			StringBuilder builder = new StringBuilder();
			var script = Database.Tables[name].Script(so);
			foreach (string s in script)
			{
				builder.AppendLine(s);
			}
			string result = builder.ToString();

			return result;
		}

		private string GetUserDefinedAggregatesText(string name)
		{
			if (Database.UserDefinedAggregates[name] == null)
				return null;

			StringBuilder builder = new StringBuilder();
			foreach (string s in Database.UserDefinedAggregates[name].Script())
			{
				builder.AppendLine(s);
			}
			string result = builder.ToString();

			return result;
		}
		
		
		private string GetUTTableText(string name)
		{
			if (name.LastIndexOf("_") == -1)
				return null;

			name = name.Replace("TT_", "");
			name = name.Substring(0, name.LastIndexOf("_"));

			if (Database.UserDefinedTableTypes[name] == null)
				return null;

			StringBuilder builder = new StringBuilder();
			foreach (string s in Database.UserDefinedTableTypes[name].Script())
			{
				builder.AppendLine(s);
			}
			string result = builder.ToString();

			return result;
		}

		private string GetTriggerText(string name)
		{
			string tableName = "";

			if (TriggersToTables.ContainsKey(name) && TriggersToTables[name] != null)
			{
				tableName = TriggersToTables[name];
			}
			else
				return null;

			foreach (Trigger tr in Database.Tables[tableName].Triggers)
			{
				if (tr.Name == name)
				{
					return tr.TextHeader + Environment.NewLine + tr.TextBody;
				}
			}

			return null;
		}

		public string GetObjectSourceText(string objName, string objType)
		{
			try
			{
				switch (objType)
				{
					case ObjType.StoredProcedure:
						return GetStoredProcedureText(objName);
					case ObjType.Table:
						return GetTableText(objName);
					case ObjType.TableTrigger:
						return GetTriggerText(objName);
					case ObjType.UserFunction:
						return GetUFText(objName);
					case ObjType.UserTableType:
						return GetUTText(objName);
					case ObjType.View:
						return GetViewText(objName);
					case ObjType.AggregateFunction:
						return GetUserDefinedAggregatesText(objName);

					//case ObjType.TableValuedFunction:
					//	return GetTableValuedFunctionText(objName);
					//case ObjType.ExtendedStoredProcedure:
					//	return GetExtendedStoredProcedureText(objName);
					//case ObjType.SqlInlineTableValuedFunction:
					//	return GetSqlInlineTableValuedFunctionText(objName);
					//case ObjType.PrimaryKeyConstraint:
					//	return GetPrimaryKeyConstraintText(objName);
					//case ObjType.CheckConstraint:
					//	return GetCheckConstraintText(objName);
					//case ObjType.DefaultConstraint:
					//	return GetDefaultConstraintText(objName);
					//case ObjType.ForeignKeyConstraint:
					//	return GetForeignKeyConstraintText(objName);
					//case ObjType.ClrScalarFunction:
					//	return GetClrScalarFunctionText(objName);
					//case ObjType.ClrStoredProcedure:
					//	return GetClrStoredProcedureText(objName);

					default:
						throw new Exception($"Object type <{objType}> is not supported!");
				}
			}
			catch
			{
			}

			return String.Empty;
		}
	}

	public enum ServerType
	{
		Main,
		Test
	}
}
