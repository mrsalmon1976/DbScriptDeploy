using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Transactions;
using DbScriptDeploy.UI.Utils;

namespace DbScriptDeploy.UI.Data
{
    public class DbHelper : IDisposable
    {
        public const int DefaultPort = 1433;
        public const string ScriptLogTable = "ScriptLog";

        private IDbConnection _conn;

        public DbHelper(DbEnvironment dbInstance)
        {
            DbInstance = dbInstance;
            _conn = GetDbConnection(dbInstance);
            _conn.Open();
        }

        public DbEnvironment DbInstance { get; set; }

        public void ArchiveLogs(IEnumerable<Guid> ids)
        {
            if (!ids.Any()) return;

            string inList = String.Join(",", ids.Select(x => String.Format("'{0}'", x)));
            using (IDbCommand cmd = _conn.CreateCommand())
            {
                string sql = String.Format("UPDATE ScriptLog SET Archived = 1 WHERE Id IN ({0})", inList);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string sql, params IDataParameter[] dbParameters)
        {
            using (IDbCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (IDataParameter parameter in dbParameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                return (T)cmd.ExecuteScalar();
            }
        }

        public void ExecuteScript(Script script)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue))
            {
				using (IDbCommand cmd = _conn.CreateCommand())
				{
					cmd.CommandText = script.ScriptText;
					cmd.ExecuteNonQuery();
				}

                using (IDbCommand cmd = _conn.CreateCommand())
                {
                    cmd.Parameters.Add(new SqlParameter("id", Guid.NewGuid()));
                    cmd.Parameters.Add(new SqlParameter("name", script.Name));
                    cmd.Parameters.Add(new SqlParameter("scriptText", script.ScriptText));
                    cmd.Parameters.Add(new SqlParameter("createdOn", DateTime.UtcNow));
                    cmd.Parameters.Add(new SqlParameter("createdUser", this.DbInstance.UserName));
                    cmd.Parameters.Add(new SqlParameter("createdAccount", AppUtils.CurrentWindowsIdentity()));

                    const string sql = "INSERT INTO ScriptLog (Id, Name, ScriptText, CreatedOn, CreatedUser, CreatedAccount) VALUES (@id, @name, @scriptText, @createdOn, @createdUser, @createdAccount)";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                scope.Complete();
            }
        }

        public IEnumerable<Script> GetExecutedScripts()
        {
            return _conn.Query<Script>("SELECT * FROM ScriptLog WHERE Archived = 0");
        }


        public void InitScriptLogTable()
        {
            using (IDbCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = Scripts.InitScriptLogTable;
                cmd.ExecuteNonQuery();
            }
        }

        public static IDbConnection GetDbConnection(DbEnvironment dbInstance)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ApplicationName = "DbScriptDeploy";

            if (dbInstance.Port != DefaultPort)
            {
                builder.DataSource = String.Format("{0}, {1}", dbInstance.Host, dbInstance.Port);
            }
            else
            {
                builder.DataSource = dbInstance.Host;
            }

            builder.InitialCatalog = dbInstance.Catalog;

            if (dbInstance.AuthType == AuthType.Windows)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = dbInstance.UserName;
                builder.Password = dbInstance.Password;
            }
            return new SqlConnection(builder.ConnectionString);
        }

        public void Dispose()
        {
            if (this._conn == null)
            {
                this._conn.Dispose();
            }
        }

        public IEnumerable<string> ParseScript(string sql)
        {
            SqlError[] errorsFound = { };
            SqlConnection conn = (SqlConnection)_conn; ;

            SqlInfoMessageEventHandler eventHandler = delegate(object sender, SqlInfoMessageEventArgs e)
            {
                errorsFound = new SqlError[e.Errors.Count];
                e.Errors.CopyTo(errorsFound, 0);
            };

            try
            {
                conn.FireInfoMessageEventOnUserErrors = true;
                conn.InfoMessage += eventHandler;

                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SET PARSEONLY ON;" + Environment.NewLine + sql + ";" + Environment.NewLine + "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.FireInfoMessageEventOnUserErrors = false;
                conn.InfoMessage -= eventHandler;
            }

            return errorsFound.Select(x => String.Format("Line {0}: [Error {1}] {2}", x.LineNumber - 1, x.Number, x.Message));
        }


    }
}
