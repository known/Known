using Known.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Known.Data
{
    public class DbContext : IDisposable
    {
        private static DbContext defaultContext = null;
        private readonly string connectionString;
        private readonly DbProviderFactory dbProviderFactory;
        private List<DbCommand> commands = new List<DbCommand>();

        public DbContext(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");

            var setting = ConfigurationManager.ConnectionStrings[connectionName];
            dbProviderFactory = DbProviderFactories.GetFactory(setting.ProviderName);
            connectionString = setting.ConnectionString;
        }

        public DbContext(string providerName, string connectionString)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            this.connectionString = connectionString;
        }

        public static string NewId
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        public static DbContext Default
        {
            get
            {
                if (defaultContext == null)
                {
                    defaultContext = new DbContext("Default");
                }
                return defaultContext;
            }
        }

        public DbDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, null);
        }

        public DbDataReader ExecuteReader(string commandText, params object[] parameters)
        {
            var connection = CreateConnection();
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, commandText, parameters);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public DataTable ExecuteTable(string commandText)
        {
            return ExecuteTable(commandText, null);
        }

        public DataTable ExecuteTable(string commandText, params object[] parameters)
        {
            var table = new DataTable("DataTable");
            using (var reader = ExecuteReader(commandText, parameters))
            {
                table.Load(reader);
            }
            return table;
        }

        public PagedResult<DataRow> ExecutePaged(string commandText, int pageSize, int pageIndex)
        {
            var source = ExecuteTable(commandText);
            var count = source.Rows.Count;
            var data = source.AsEnumerable().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return new PagedResult<DataRow>(count, data);
        }

        public DataRow ExecuteRow(string commandText)
        {
            return ExecuteRow(commandText, null);
        }

        public DataRow ExecuteRow(string commandText, params object[] parameters)
        {
            var data = ExecuteTable(commandText, parameters);
            return data.Rows.Count > 0 ? data.Rows[0] : null;
        }

        public T ExecuteScalar<T>(string commandText)
        {
            return ExecuteScalar<T>(commandText, null);
        }

        public T ExecuteScalar<T>(string commandText, params object[] parameters)
        {
            var result = default(T);
            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, commandText, parameters);
                var value = command.ExecuteScalar();
                result = value.To<T>();
            }
            return result;
        }

        public string ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, null);
        }

        public string ExecuteNonQuery(string commandText, params object[] parameters)
        {
            ExecuteOnSubmit(commandText, parameters);
            return SubmitChanges();
        }

        public void ExecuteOnSubmit(string commandText)
        {
            ExecuteOnSubmit(commandText, null);
        }

        public void ExecuteOnSubmit(string commandText, params object[] parameters)
        {
            var command = dbProviderFactory.CreateCommand();
            PrepareCommand(command, commandText, parameters);
            commands.Add(command);
        }

        public string SubmitChanges()
        {
            if (commands.Count == 0)
                return string.Empty;

            var result = string.Empty;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        commands.ForEach(c =>
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
                        commands.Clear();
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            commands.Clear();
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            if (commands.Count > 0)
            {
                var texts = commands.Select(c => c.CommandText).ToArray();
                return string.Join(Environment.NewLine, texts);
            }
            return string.Empty;
        }

        public string ToSqlString()
        {
            var sb = new StringBuilder();
            commands.ForEach(c =>
            {
                var text = c.CommandText;
                c.Parameters.ForEach<DbParameter>(p =>
                {
                    var value = string.Format("'{0}'", p.Value);
                    text = text.Replace(p.ParameterName, value);
                });
                sb.AppendLine(text);
            });
            return sb.ToString();
        }

        private DbConnection CreateConnection()
        {
            var connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        private static void PrepareCommand(DbCommand command, string commandText, object[] parameterValues)
        {
            var parameters = new Dictionary<string, object>();
            if (parameterValues != null && parameterValues.Length > 0)
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    var name = string.Format("P{0}", i);
                    parameters.Add(name, parameterValues[i]);
                }
            }
            PrepareCommand(command, commandText, parameters);
        }

        private static void PrepareCommand(DbCommand command, string commandText, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new InvalidOperationException("CommandText must be not null.");

            command.CommandText = commandText.Trim();
            var isOracleCommand = command.GetType().Name == "OracleCommand";
            if (isOracleCommand)
                command.CommandText.Replace("+", "||");

            if (parameters != null && parameters.Count > 0)
            {
                string prefix = "@";
                if (command is SqlCommand)
                    prefix = "@";
                else if (isOracleCommand)
                    prefix = ":";
                command.CommandText = command.CommandText.Replace("?", prefix);
                foreach (string key in parameters.Keys)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = prefix + key.TrimStart('@', ':');
                    parameter.Value = FormatValue(parameters[key]);
                    command.Parameters.Add(parameter);
                }
                parameters.Clear();
            }

            if (command.Connection != null && command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
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
