using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// 数据库接口。
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 取得数据库类型。
        /// </summary>
        DatabaseType Type { get; }

        /// <summary>
        /// 取得数据库连接字符串。
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 执行增删改数据库命令。
        /// </summary>
        /// <param name="command">增删改数据库命令。</param>
        void Execute(Command command);

        /// <summary>
        /// 执行增删改数据库命令集合。
        /// </summary>
        /// <param name="commands">增删改数据库命令集合。</param>
        void Execute(List<Command> commands);

        /// <summary>
        /// 执行查询数据库命令，返回标量。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns></returns>
        object Scalar(Command command);

        /// <summary>
        /// 执行查询数据库命令，返回查询结果集。
        /// </summary>
        /// <param name="command">查询数据库命令。</param>
        /// <returns>查询结果集。</returns>
        DataTable Query(Command command);

        /// <summary>
        /// 将整表数据写入数据库，表名及栏位名需与数据库一致。
        /// </summary>
        /// <param name="table">数据表。</param>
        void WriteTable(DataTable table);
    }
}
