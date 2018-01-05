using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Known.Data
{
    /// <summary>
    /// 数据库访问异常。
    /// </summary>
    public class DatabaseException : DataException
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="command">发生异常时的数据库命令。</param>
        public DatabaseException(List<Command> commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="command">发生异常时的数据库命令。</param>
        /// <param name="message">当引发异常时显示的字符串。</param>
        public DatabaseException(List<Command> commands, string message) 
            : base(message)
        {
            Commands = commands;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="command">发生异常时的数据库命令。</param>
        /// <param name="message">当引发异常时显示的字符串。</param>
        /// <param name="innerException">对内部异常的引用。</param>
        public DatabaseException(List<Command> commands, string message, Exception innerException)
            : base(message, innerException)
        {
            Commands = commands;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="info">将对象序列化或反序列化所必需的数据。</param>
        /// <param name="context">指定序列化流的源和目的地的说明。</param>
        protected DatabaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 获取数据库发生异常时的数据库命令集合。
        /// </summary>
        public List<Command> Commands { get; }
    }
}
