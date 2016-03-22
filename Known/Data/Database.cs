using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known.Data
{
    public interface IDatabase
    {
        IEnumerable<T> ExecuteReader<T>(string sql, object parameters, Func<DbDataReader, T> action);
        int ExecuteNonQuery(string sql, object parameters);
        void ExecuteTransaction(Action<IDatabase> action);
    }

    public class DatabaseInTx : IDatabase
    {
        private readonly DbCommand _command;

        public DatabaseInTx(DbCommand command)
        {
            _command = command;
        }

        private void PrepareCommand(string sql, object parameters)
        {
            _command.CommandType = CommandType.Text;
            _command.CommandText = sql;
            _command.Parameters.Clear();
            if (parameters != null)
            {
                var type = parameters.GetType();
                var properties = type.GetProperties();
                foreach (var pi in properties)
                {
                    var parameter = _command.CreateParameter();
                    parameter.ParameterName = pi.Name;
                    parameter.Value = pi.GetValue(parameters, null) ?? DBNull.Value;
                    _command.Parameters.Add(parameter);
                }
            }
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, object parameters, Func<DbDataReader, T> action)
        {
            PrepareCommand(sql, parameters);
            using (var dr = _command.ExecuteReader())
            {
                while (dr.Read())
                    yield return action.Invoke(dr);
            }
        }

        public int ExecuteNonQuery(string sql, object parameters)
        {
            PrepareCommand(sql, parameters);
            return _command.ExecuteNonQuery();
        }

        public void ExecuteTransaction(Action<IDatabase> action)
        {
            if (action != null)
                action.Invoke(this);
        }
    }

    public class Database : IDatabase
    {
        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        public Database(string connectionStringName)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            var connectionString = connectionStringSettings.ConnectionString;
            _connectionString = connectionString;
            _providerFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
        }

        private static IDatabase _default = null;
        public static IDatabase Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Database("Default");
                }
                return _default;
            }
        }

        private DbConnection CreateConnection()
        {
            var connection = _providerFactory.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, object parameters, Func<DbDataReader, T> action)
        {
            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                var db = new DatabaseInTx(cmd);
                foreach (var item in db.ExecuteReader(sql, parameters, action))
                    yield return item;
            }
        }

        public int ExecuteNonQuery(string sql, object parameters)
        {
            using (var connection = CreateConnection())
            using (var cmd = connection.CreateCommand())
            {
                var db = new DatabaseInTx(cmd);
                return db.ExecuteNonQuery(sql, parameters);
            }
        }

        public void ExecuteTransaction(Action<IDatabase> action)
        {
            using (var connection = CreateConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;
                        var db = new DatabaseInTx(cmd);
                        db.ExecuteTransaction(action);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
