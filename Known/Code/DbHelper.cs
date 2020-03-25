using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known
{
    public sealed class DbHelper
    {
        public static DbConnection CreateConnection(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "Default";

            var setting = ConfigurationManager.ConnectionStrings[name];
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);
            var conn = factory.CreateConnection();
            conn.ConnectionString = setting.ConnectionString;
            return conn;
        }

        public static int ExecuteNonQuery(DbConnection conn, string sql, IDictionary<string, object> parameters = null, DbTransaction trans = null)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));

            var cmd = conn.CreateCommand();
            PrepareCommand(conn, cmd, trans, sql, parameters, out bool closeConn);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (closeConn)
                conn.Close();

            return value;
        }

        public static object ExecuteScalar(DbConnection conn, string sql, IDictionary<string, object> parameters = null, DbTransaction trans = null)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));

            var cmd = conn.CreateCommand();
            PrepareCommand(conn, cmd, trans, sql, parameters, out bool closeConn);
            var scalar = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (closeConn)
                conn.Close();

            return scalar;
        }

        public static DbDataReader ExecuteReader(DbConnection conn, string sql, IDictionary<string, object> parameters = null, DbTransaction trans = null)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));

            var closeConn = false;
            var cmd = conn.CreateCommand();

            try
            {
                PrepareCommand(conn, cmd, trans, sql, parameters, out closeConn);
                var reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                if (closeConn)
                    conn.Close();
                throw;
            }
        }

        private static void PrepareCommand(DbConnection conn, DbCommand cmd, DbTransaction trans, string sql, IDictionary<string, object> parameters, out bool closeConn)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            if (sql == null || sql.Length == 0)
                throw new ArgumentNullException(nameof(sql));

            if (conn.State != ConnectionState.Open)
            {
                closeConn = true;
                conn.Open();
            }
            else
            {
                closeConn = false;
            }

            var isOracle = conn.GetType().Name.Contains("Oracle");
            var prefix = isOracle ? ":" : "@";

            cmd.Connection = conn;
            cmd.CommandText = isOracle ? sql.Replace("@", ":") : sql;

            if (trans != null)
            {
                if (trans.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof(trans));
                cmd.Transaction = trans;
            }

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = $"{prefix}{item.Key}";
                    p.Value = item.Value;
                    cmd.Parameters.Add(p);
                }
            }
        }
    }
}