using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// Dapper数据访问提供者。
    /// </summary>
    public class DapperProvider : IProvider
    {
        private IDbConnection connection;

        /// <summary>
        /// 构造函数，创建一个Dapper数据访问提供者示例。
        /// </summary>
        /// <param name="connection">数据库连接对象。</param>
        public DapperProvider(IDbConnection connection)
        {
            this.connection = connection;
            ConnectionString = connection.ConnectionString;
        }

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
                var sql = command.Text;
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                connection.Execute(sql, param);
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
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            var sql = command.Text;
                            var param = GetDynamicParameters(command.Parameters);
                            connection.Execute(sql, param, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
                var sql = command.Text;
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                return connection.ExecuteScalar(sql, param);
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
                var table = new DataTable("Table");
                var sql = command.Text;
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                using (var reader = connection.ExecuteReader(sql, param))
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

        private DynamicParameters GetDynamicParameters(Dictionary<string, object> parameters)
        {
            var dynamicParameters = new DynamicParameters();
            foreach (var item in parameters)
            {
                dynamicParameters.Add(item.Key, item.Value);
            }
            return dynamicParameters;
        }
    }
}
