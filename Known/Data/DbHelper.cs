using Known.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Known.Data
{
    public class DbHelper : IDisposable
    {
        private static DbHelper _default = null;
        private readonly string connectionString;
        private readonly DbProviderFactory dbProviderFactory;
        private List<DbCommand> dbCommands = new List<DbCommand>();

        public DbHelper(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");

            var setting = ConfigurationManager.ConnectionStrings[connectionName];
            dbProviderFactory = DbProviderFactories.GetFactory(setting.ProviderName);
            connectionString = setting.ConnectionString;
        }

        public static string NewId
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        public static DbHelper Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new DbHelper("Default");
                }
                return _default;
            }
        }

        public Command CreateCommand()
        {
            return new Command(this);
        }

        public DataTable ExecuteTable(string commandText, Dictionary<string, object> parameters)
        {
            var table = new DataTable("DataTable");
            var connection = CreateConnection();
            using (var dbCommand = connection.CreateCommand())
            {
                PrepareCommand(dbCommand, commandText, parameters);
                using (var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        public PagedResult<DataRow> ExecutePaged(string commandText, Dictionary<string, object> parameters, int pageSize, int pageIndex)
        {
            var source = ExecuteTable(commandText, parameters);
            var count = source.Rows.Count;
            var data = source.AsEnumerable().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return new PagedResult<DataRow>(count, data);
        }

        public DataRow ExecuteRow(string commandText, Dictionary<string, object> parameters)
        {
            var data = ExecuteTable(commandText, parameters);
            return data.Rows.Count > 0 ? data.Rows[0] : null;
        }

        public T ExecuteScalar<T>(string commandText, Dictionary<string, object> parameters)
        {
            var result = default(T);
            using (var connection = CreateConnection())
            using (var dbCommand = connection.CreateCommand())
            {
                PrepareCommand(dbCommand, commandText, parameters);
                var value = dbCommand.ExecuteScalar();
                result = value.To<T>();
            }
            return result;
        }

        public string Execute(string commandText, Dictionary<string, object> parameters)
        {
            ExecuteOnSubmit(commandText, parameters);
            return SubmitChanges();
        }

        public void ExecuteOnSubmit(string commandText, Dictionary<string, object> parameters)
        {
            var dbCommand = dbProviderFactory.CreateCommand();
            PrepareCommand(dbCommand, commandText, parameters);
            dbCommands.Add(dbCommand);
        }

        public string SubmitChanges()
        {
            if (dbCommands.Count == 0)
                return null;

            string result = null;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        dbCommands.ForEach(c =>
                        {
                            c.Connection = connection;
                            c.Transaction = transaction;
                            c.ExecuteNonQuery();
                        });
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        dbCommands.Clear();
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            dbCommands.Clear();
            GC.SuppressFinalize(this);
        }

        private static void PrepareCommand(DbCommand dbCommand, string commandText, Dictionary<string, object> parameters)
        {
            dbCommand.CommandText = commandText.Trim();
            var isOracleCommand = dbCommand.GetType().Name == "OracleCommand";
            if (isOracleCommand)
                dbCommand.CommandText.Replace("+", "||");//Oracle链接字符为||

            if (parameters != null && parameters.Count > 0)
            {
                string prefix = "@";
                if (dbCommand is SqlCommand)
                    prefix = "@";
                else if (isOracleCommand)
                    prefix = ":";
                dbCommand.CommandText = dbCommand.CommandText.Replace("?", prefix);//替换?

                parameters.ForEach(p =>
                {
                    var parameter = dbCommand.CreateParameter();
                    parameter.ParameterName = prefix + p.Key.TrimStart('@', ':');
                    parameter.Value = FormatValue(p.Value);
                    dbCommand.Parameters.Add(parameter);
                });
            }

            if (dbCommand.Connection != null && dbCommand.Connection.State != ConnectionState.Open)
                dbCommand.Connection.Open();
        }

        private DbConnection CreateConnection()
        {
            var connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        private static object FormatValue(object value)
        {
            if (value == null)
                return DBNull.Value;

            var valueString = value.ToString();
            if (string.IsNullOrEmpty(valueString))
                return DBNull.Value;

            return valueString;
        }
    }
}
