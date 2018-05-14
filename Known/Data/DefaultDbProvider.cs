using System;
using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// 默认数据访问提供者。
    /// </summary>
    public class DefaultDbProvider : IDbProvider
    {
        private IDbConnection connection;

        /// <summary>
        /// 构造函数，创建一个数据访问提供者实例。
        /// </summary>
        /// <param name="connection">数据库连接对象。</param>
        /// <param name="providerName">数据库提供者名称。</param>
        public DefaultDbProvider(IDbConnection connection, string providerName)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ProviderName = providerName;
            ConnectionString = connection.ConnectionString;
        }

        /// <summary>
        /// 取得数据库提供者名称。
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// 取得数据库连接字符串。
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 执行增删改数据库命令。
        /// </summary>
        /// <param name="command">增删改数据库命令。</param>
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

        /// <summary>
        /// 执行增删改数据库命令集合。
        /// </summary>
        /// <param name="commands">增删改数据库命令集合。</param>
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

        /// <summary>
        /// 执行查询数据库命令，返回标量。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns></returns>
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

        /// <summary>
        /// 执行查询数据库命令，返回查询结果集。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns>查询结果集。</returns>
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

        /// <summary>
        /// 将整表数据写入数据库，表名及栏位名需与数据库一致。
        /// </summary>
        /// <param name="table">数据表。</param>
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
