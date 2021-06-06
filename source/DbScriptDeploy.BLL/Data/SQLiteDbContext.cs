﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DbScriptDeploy.BLL.Data
{
    public class SQLiteDbContext : DbContext
    {
        private readonly string _dbPath;
        private readonly string _connString;
        private SQLiteConnection _conn;

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public SQLiteDbContext(string filePath) : base()
        {
            _dbPath = filePath;
            _connString = String.Format("Data Source={0};Version=3;", filePath);
            _conn = new SQLiteConnection(_connString);

            string dir = new FileInfo(_dbPath).Directory.FullName;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!System.IO.File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            _conn.Open();
        }


        /// <summary>
        /// Begins a new transaction (if supported by the DbContext)
        /// </summary>
        public override void BeginTransaction()
        {
            this.Transaction = _conn.BeginTransaction();
        }

        /// <summary>
        /// Commits the current transaction (if supported by the DbContext)
        /// </summary>
        public override void Commit()
        {
            if (this.Transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }
            this.Transaction.Commit();
        }

        public override void Dispose()
        {
            if (_conn != null)
            {
                _conn.Dispose();
            }
        }

        /// <summary>
        /// Executes a query and maps the result to a strongly typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return _conn.Query<T>(sql, param, this.Transaction);
        }

        /// <summary>
        /// Executes a query against the database
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public override void ExecuteNonQuery(string sql, params IDbDataParameter[] dbParameters)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            {
                cmd.Parameters.AddRange(dbParameters);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a query against the database
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public override T ExecuteScalar<T>(string sql, object param = null)
        {
            return _conn.ExecuteScalar<T>(sql, param, this.Transaction);
        }

        /// <summary>
        /// Executes a query against the database using Dapper to substitute model values.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public override void ExecuteNonQuery(string sql, object param)
        {
            _conn.Execute(sql, param, this.Transaction);
        }

        /// <summary>
        /// Initialises the application database.  Creates a new file and all tables if they do not exist.
        /// </summary>
        public override void Initialise()
        {
            string sql = this.ReadResource("DbScriptDeploy.BLL.Data.Scripts.Identity.sql");
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            {
                cmd.ExecuteNonQuery();
            }

            sql = this.ReadResource("DbScriptDeploy.BLL.Data.Scripts.SQLite.sql");
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Rolls back the current transaction (if supported by the DbContext)
        /// </summary>
        public override void Rollback()
        {
            if (this.Transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }
            this.Transaction.Rollback();
        }

    }
}
