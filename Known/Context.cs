using Known.Data;
using Known.Log;

namespace Known
{
    /// <summary>
    /// 上下文基类。
    /// </summary>
    public class Context
    {
        /// <summary>
        /// 构造函数，创建上下文实例。
        /// </summary>
        /// <param name="logger">日志组件。</param>
        public Context(ILogger logger) : this(null, logger) { }

        /// <summary>
        /// 构造函数，创建上下文实例。
        /// </summary>
        /// <param name="database">数据访问组件。</param>
        /// <param name="logger">日志组件。</param>
        public Context(Database database, ILogger logger) : this(database, logger, null) { }

        /// <summary>
        /// 构造函数，创建上下文实例。
        /// </summary>
        /// <param name="database">数据访问组件。</param>
        /// <param name="logger">日志组件。</param>
        /// <param name="userName">当前用户账号。</param>
        public Context(Database database, ILogger logger, string userName)
        {
            Database = database;
            if (Database != null)
            {
                Database.UserName = userName;
            }

            Logger = logger;
            UserName = userName;
        }

        /// <summary>
        /// 取得数据访问组件。
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// 取得日志组件。
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// 取得当前用户账号。
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// 取得或设置上下文扩展参数。
        /// </summary>
        public dynamic Param { get; set; }
    }
}
