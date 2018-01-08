using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Known.Data
{
    /// <summary>
    /// SqlServer数据库访问类。
    /// </summary>
    public class SqlServerDatabase : IDatabase
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public SqlServerDatabase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 取得数据库类型。
        /// </summary>
        public DatabaseType Type
        {
            get { return DatabaseType.SqlServer; }
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
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    PrepareCommand(cmd, null, command);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 执行增删改数据库命令集合。
        /// </summary>
        /// <param name="commands">增删改数据库命令集合。</param>
        public void Execute(List<Command> commands)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    var cmd = conn.CreateCommand();
                    try
                    {
                        foreach (var command in commands)
                        {
                            PrepareCommand(cmd, trans, command);
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        cmd.Dispose();
                        throw new DatabaseException(commands, ex.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询数据库命令，返回标量。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns></returns>
        public object Scalar(Command command)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    PrepareCommand(cmd, null, command);
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 执行查询数据库命令，返回查询结果集。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns>查询结果集。</returns>
        public DataTable Query(Command command)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    PrepareCommand(cmd, null, command);
                    var table = new DataTable("Table");
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
            }
        }

        /// <summary>
        /// 将整表数据写入数据库，表名及栏位名需与数据库一致。
        /// </summary>
        /// <param name="table">数据表。</param>
        public void WriteTable(DataTable table)
        {
            var bulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.UseInternalTransaction)
            {
                BulkCopyTimeout = 300,
                DestinationTableName = table.TableName
            };
            var dt = new DataTable(table.TableName);
            using (var adapter = new SqlDataAdapter($"select top 0 * from {table.TableName}", ConnectionString))
            {
                adapter.Fill(dt);
            }
            foreach (DataColumn item in table.Columns)
            {
                var sourceColumn = item.ColumnName;
                var destinationColumn = dt.Columns[sourceColumn].ColumnName;
                bulkCopy.ColumnMappings.Add(sourceColumn, destinationColumn);
            }
            bulkCopy.WriteToServer(table);
        }

        private void PrepareCommand(SqlCommand cmd, SqlTransaction trans, Command command, CommandType cmdType = CommandType.Text)
        {
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandText = command.Text;
            cmd.CommandType = cmdType;
            if (command.HasParameter)
            {
                cmd.Parameters.Clear();
                foreach (var param in command.Parameters)
                {
                    cmd.Parameters.Add(new SqlParameter("@" + param.Key, param.Value));
                }
            }
        }
    }
}
