using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known.Data
{
    class DbProvider : IDbProvider, IDisposable
    {
        private readonly DbProviderFactory factory;
        private IDbConnection conn;
        private IDbTransaction trans;

        public DbProvider(string name)
        {
            Name = name;
            var setting = ConfigurationManager.ConnectionStrings[name];
            factory = DbProviderFactories.GetFactory(setting.ProviderName);
            ProviderName = setting.ProviderName;
            ConnectionString = setting.ConnectionString;
        }

        public string Name { get; }
        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void BeginTrans()
        {
            CreateConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();

            trans = conn.BeginTransaction();
        }

        public void Commit()
        {
            if (trans != null)
                trans.Commit();
        }

        public void Rollback()
        {
            if (trans != null)
                trans.Rollback();
        }

        public void Execute(Command command)
        {
            try
            {
                var cmd = CreateDbCommand(command);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var commands = new List<Command> { command };
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                DisposeConnection();
            }
        }

        public object Scalar(Command command)
        {
            try
            {
                var cmd = CreateDbCommand(command);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                var commands = new List<Command> { command };
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                DisposeConnection();
            }
        }

        public DataTable Query(Command command)
        {
            try
            {
                var table = new DataTable();
                var cmd = CreateDbCommand(command);
                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
                return table;
            }
            catch (Exception ex)
            {
                var commands = new List<Command> { command };
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                DisposeConnection();
            }
        }

        public void WriteTable(DataTable table)
        {
            var command = CommandHelper.GetInsertCommand(table);

            try
            {
                var cmd = CreateDbCommand(command);
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn item in table.Columns)
                    {
                        command.Parameters.Add(item.ColumnName, row[item]);
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = item.ColumnName;
                        parameter.Value = FormatValue(row[item]);
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                var commands = new List<Command> { command };
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                DisposeConnection();
            }
        }

        public void Dispose()
        {
            if (trans != null)
                trans.Dispose();

            trans = null;

            DisposeConnection();
        }

        private void CreateConnection()
        {
            if (conn == null)
            {
                conn = factory.CreateConnection();
                conn.ConnectionString = ConnectionString;
            }
        }

        private void DisposeConnection()
        {
            if (trans != null)
                return;

            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                conn.Dispose();
                conn = null;
            }
        }

        private IDbCommand CreateDbCommand(Command command)
        {
            CreateConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = command.Text;
            if (ProviderName.Contains("Oracle"))
                cmd.CommandText = cmd.CommandText.Replace("@", ":");

            if (trans != null)
                cmd.Transaction = trans;

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

            if (conn.State != ConnectionState.Open)
                conn.Open();

            return cmd;
        }

        private static object FormatValue(object value)
        {
            if (value == null)
                return "";

            var valueType = value.GetType();
            if (valueType == typeof(string))
            {
                var valueString = value.ToString().Trim();
                if (valueString.Length == 0)
                    return "";

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
