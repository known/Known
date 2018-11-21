using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known.Data
{
    public class DbProvider : IDbProvider
    {
        private readonly DbProviderFactory factory;

        public DbProvider(string name)
        {
            var setting = ConfigurationManager.ConnectionStrings[name];
            factory = DbProviderFactories.GetFactory(setting.ProviderName);
            ProviderName = setting.ProviderName;
            ConnectionString = setting.ConnectionString;
        }

        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void Execute(Command command)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;

            try
            {
                conn = CreateAndOpenConnection();
                cmd = CreateDbCommand(conn, command);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                DisposeDbObject(conn, cmd);
            }
        }

        public void Execute(List<Command> commands)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;
            IDbTransaction trans = null;

            try
            {
                conn = CreateAndOpenConnection();
                trans = conn.BeginTransaction();
                foreach (var command in commands)
                {
                    cmd = CreateDbCommand(conn, command);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                DisposeDbObject(conn, cmd);
                if (trans != null)
                    trans.Dispose();
            }
        }

        public object Scalar(Command command)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;

            try
            {
                conn = CreateAndOpenConnection();
                cmd = CreateDbCommand(conn, command);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                DisposeDbObject(conn, cmd);
            }
        }

        public DataTable Query(Command command)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;

            try
            {
                var table = new DataTable();
                conn = CreateAndOpenConnection();
                cmd = CreateDbCommand(conn, command);
                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
                return table;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                DisposeDbObject(conn, cmd);
            }
        }

        public void WriteTable(DataTable table)
        {
            IDbConnection conn = null;
            IDbCommand cmd = null;

            try
            {
                conn = CreateAndOpenConnection();
                var command = CommandCache.GetInsertCommand(table);
                cmd = conn.CreateCommand();
                cmd.CommandText = GetCommandText(command);
                foreach (DataRow row in table.Rows)
                {
                    PrepareCommandParameters(cmd, row);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataException(ex.Message, ex);
            }
            finally
            {
                DisposeDbObject(conn, cmd);
            }
        }

        private IDbConnection CreateAndOpenConnection()
        {
            var conn = factory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            return conn;
        }

        private IDbCommand CreateDbCommand(IDbConnection conn, Command command)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = GetCommandText(command);
            if (command.HasParameter)
            {
                foreach (var item in command.Parameters)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = FormatValue(item.Value);
                    cmd.Parameters.Add(parameter);
                }
            }

            return cmd;
        }

        private static void DisposeDbObject(IDbConnection conn, IDbCommand cmd)
        {
            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
            }

            if (cmd != null)
            {
                cmd.Dispose();
            }
        }

        private string GetCommandText(Command command)
        {
            if (ProviderName.Contains("Oracle"))
            {
                return command.Text.Replace("@", ":");
            }
            return command.Text;
        }

        private void PrepareCommandParameters(IDbCommand cmd, DataRow row)
        {
            foreach (DataColumn item in row.Table.Columns)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = item.ColumnName;
                parameter.Value = FormatValue(row[item]);
                cmd.Parameters.Add(parameter);
            }
        }

        private static object FormatValue(object value)
        {
            if (value == null)
                return DBNull.Value;

            var valueType = value.GetType();
            if (valueType == typeof(string))
            {
                var valueString = value.ToString().Trim();
                if (valueString.Length == 0)
                    return DBNull.Value;

                return valueString;
            }

            if (valueType.IsEnum)
                return (int)value;

            if (valueType == typeof(bool))
                return (bool)value ? 1 : 0;

            return value;
        }
    }
}
