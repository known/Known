using Known.Data;
using Known.Log;
using System.Collections.Generic;

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
        /// <param name="database">数据访问组件。</param>
        /// <param name="logger">日志组件。</param>
        public Context(Database database, ILogger logger)
        {
            Database = database;
            Logger = logger;
            Params = new Dictionary<string, object>();
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
        /// 取得上下文扩展参数字典。
        /// </summary>
        public IDictionary<string, object> Params { get; }
    }
}
