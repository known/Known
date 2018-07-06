using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known.Data
{
    public class DefaultDbProvider : IDbProvider
    {
        private IDbConnection connection;

        public DefaultDbProvider(string name)
        {
            var setting = ConfigurationManager.ConnectionStrings[name];
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);
            connection = factory.CreateConnection();
            connection.ConnectionString = setting.ConnectionString;
            ProviderName = setting.ProviderName;
            ConnectionString = setting.ConnectionString;
        }

        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void Execute(Command command)
        {
            try
            {
                OpenConnection();
                using (var cmd = GetDbCommand(command))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Execute(List<Command> commands)
        {
            try
            {
                OpenConnection();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            var cmd = GetDbCommand(command);
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new DatabaseException(commands, ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public object Scalar(Command command)
        {
            try
            {
                OpenConnection();
                using (var cmd = GetDbCommand(command))
                {
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable Query(Command command)
        {
            try
            {
                var table = new DataTable();
                OpenConnection();
                using (var cmd = GetDbCommand(command))
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
                CloseConnection();
            }
        }

        public void WriteTable(DataTable table)
        {
            try
            {
                OpenConnection();
                var command = CommandCache.GetInsertCommand(table);
                var cmd = connection.CreateCommand();
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
                CloseConnection();
            }
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
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

        private IDbCommand GetDbCommand(Command command)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = GetCommandText(command);
            if (command.HasParameter)
            {
                foreach (var item in command.Parameters)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value;
                    cmd.Parameters.Add(parameter);
                }
            }

            return cmd;
        }

        private void PrepareCommandParameters(IDbCommand cmd, DataRow row)
        {
            foreach (DataColumn item in row.Table.Columns)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = item.ColumnName;
                parameter.Value = row[item];
                cmd.Parameters.Add(parameter);
            }
        }
    }
}
