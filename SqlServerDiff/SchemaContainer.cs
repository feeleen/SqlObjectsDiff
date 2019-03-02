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
			
		public SchemaContainer(ServerType servertype)
		{
			conn.ServerInstance = ServerName;
			conn.LoginSecure = false;
			conn.ConnectionString = GetSSPIConnectionString(ServerName, DBName);
			myServer = new Server(conn);
			this.servertype = servertype;
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
					throw new Exception("Main database name is not correct!");

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

		public string ServerName
		{
			get { return ConfigurationManager.AppSettings[ServerTypeName + "Server"].Replace(" ", ""); }
		}
		
		public string DBName
		{
			get { return ConfigurationManager.AppSettings[ServerTypeName + "Database"].Replace(" ", ""); }
		}
		
		/*
		public Dictionary<string, string> UserTableTypes = new Dictionary<string, string>();
		public Dictionary<string, string> Tables = new Dictionary<string, string>();
		public Dictionary<string, string> TableTriggers = new Dictionary<string, string>();
		public Dictionary<string, string> UserFunctions = new Dictionary<string, string>();
		public Dictionary<string, string> StoredProcedures = new Dictionary<string, string>();
		public Dictionary<string, string> Views = new Dictionary<string, string>();
		*/
		public Dictionary<string, string> TriggersToTables = new Dictionary<string, string>();
		public Dictionary<string, string> TableText = new Dictionary<string, string>();
		public Dictionary<string, string> SPText= new Dictionary<string, string>();
		public Dictionary<string, string> TriggerText= new Dictionary<string, string>();
		public Dictionary<string, string> ViewText= new Dictionary<string, string>();
		public Dictionary<string, string> UFText= new Dictionary<string, string>();
		public Dictionary<string, string> UTText= new Dictionary<string, string>();
		

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

		public Dictionary<string, string> GetChangedObjects(int days)
		{
			string sql = $@"use {Database.Name};
                select
                    name,
                    type,
                    type_desc
                from sys.all_objects
                where modify_date > dateadd(d, -{days}, getdate())
                order by modify_date desc";

			DataSet set = Database.ExecuteWithResults(sql);

			Dictionary<string, string> objects = new Dictionary<string, string>();

			foreach (DataTable t in set.Tables)
			{
				foreach (DataRow row in t.Rows)
					objects[row["name"].ToString()] = row["type"].ToString();
				break;
			}

			return objects;
		}

		public string GetViewText(string name)
		{
			if (Database.Views[name] == null)
				return null;

			return Database.Views[name].TextHeader + Environment.NewLine + Database.Views[name].TextBody;
		}

		public string GetStoredProcedureText(string name)
		{
			if (Database.StoredProcedures[name] == null)
				return null;

			return Database.StoredProcedures[name].TextHeader + Environment.NewLine + Database.StoredProcedures[name].TextBody;
		}

		public string GetUTText(string name)
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

		public string GetUFText(string name)
		{
			if (Database.UserDefinedFunctions[name] == null)
				return null;

			return Database.UserDefinedFunctions[name].TextHeader + Environment.NewLine + Database.UserDefinedFunctions[name].TextBody;
		}

		public string GetTableText(string name)
		{
			if (Database.Tables[name] == null)
				return null;

			StringBuilder builder = new StringBuilder();
			foreach (string s in Database.Tables[name].Script())
			{
				builder.AppendLine(s);
			}
			string result = builder.ToString();

			return result;
		}


		public string GetUTTableText(string name)
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

		public string GetTriggerText(string name)
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
	}

	public enum ServerType
	{
		Main,
		Test
	}
}
