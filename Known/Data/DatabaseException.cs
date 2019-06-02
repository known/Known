using System;
using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// 数据库访问异常类。
    /// </summary>
    public class DatabaseException : DataException
    {
        /// <summary>
        /// 初始化一个数据库访问异常类实例。
        /// </summary>
        /// <param name="commands">异常命令集合。</param>
        public DatabaseException(List<Command> commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// 初始化一个数据库访问异常类实例。
        /// </summary>
        /// <param name="commands">异常命令集合。</param>
        /// <param name="message">异常提示信息。</param>
        public DatabaseException(List<Command> commands, string message) 
            : base(message)
        {
            Commands = commands;
        }

        /// <summary>
        /// 初始化一个数据库访问异常类实例。
        /// </summary>
        /// <param name="commands">异常命令集合。</param>
        /// <param name="message">异常提示信息。</param>
        /// <param name="innerException">内部异常对象。</param>
        public DatabaseException(List<Command> commands, string message, Exception innerException)
            : base(message, innerException)
        {
            Commands = commands;
        }

        /// <summary>
        /// 取得数据库访问异常命令集合。
        /// </summary>
        public List<Command> Commands { get; }
    }
}
