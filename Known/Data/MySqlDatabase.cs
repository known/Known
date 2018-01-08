using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// MySql数据库访问类。
    /// </summary>
    public class MySqlDatabase : IDatabase
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MySqlDatabase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 取得数据库类型。
        /// </summary>
        public DatabaseType Type
        {
            get { return DatabaseType.MySql; }
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
            using (var conn = new MySqlConnection(ConnectionString))
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
            using (var conn = new MySqlConnection(ConnectionString))
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
            using (var conn = new MySqlConnection(ConnectionString))
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
            using (var conn = new MySqlConnection(ConnectionString))
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
        }

        private void PrepareCommand(MySqlCommand cmd, MySqlTransaction trans, Command command, CommandType cmdType = CommandType.Text)
        {
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandText = command.Text.Replace("@", "?");
            cmd.CommandType = cmdType;
            if (command.HasParameter)
            {
                cmd.Parameters.Clear();
                foreach (var param in command.Parameters)
                {
                    cmd.Parameters.Add(new MySqlParameter("?" + param.Key, param.Value));
                }
            }
        }
    }
}
