using System;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// 数据库访问提供者接口。
    /// </summary>
    public interface IDbProvider : IDisposable
    {
        /// <summary>
        /// 取得数据库访问链接名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得数据库访问提供者名称。
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 取得数据库访问链接字符串。
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 开始数据库事务。
        /// </summary>
        void BeginTrans();

        /// <summary>
        /// 提交数据库事务。
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚数据库事务。
        /// </summary>
        void Rollback();

        /// <summary>
        /// 执行增删改等非查询操作的命令。
        /// </summary>
        /// <param name="command">数据库操作命令。</param>
        void Execute(Command command);

        /// <summary>
        /// 执行数据库查询命令，返回标量值。
        /// </summary>
        /// <param name="command">数据库查询命令。</param>
        /// <returns>标量值。</returns>
        object Scalar(Command command);

        /// <summary>
        /// 执行数据库查询命令，返回数据表。
        /// </summary>
        /// <param name="command">数据库查询命令。</param>
        /// <returns>数据表。</returns>
        DataTable Query(Command command);

        /// <summary>
        /// 批量将数据表数据写入数据库。
        /// </summary>
        /// <param name="table">数据表。</param>
        void WriteTable(DataTable table);
    }
}
