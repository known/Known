using System.Collections.Generic;
using System.Dynamic;
using Known.Data;
using Known.Log;

namespace Known
{
    /// <summary>
    /// 应用程序上下文类型。
    /// </summary>
    public class Context
    {
        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        public Context(ILogger logger) : this(null, logger) { }

        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="database">数据库访问对象。</param>
        /// <param name="logger">日志对象。</param>
        public Context(Database database, ILogger logger) : this(database, logger, null) { }

        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="database">数据库访问对象。</param>
        /// <param name="logger">日志对象。</param>
        /// <param name="userName">当前用户名。</param>
        public Context(Database database, ILogger logger, string userName)
        {
            if (database != null)
            {
                database.UserName = userName;
            }

            Database = database;
            Logger = logger;
            UserName = userName;
            Parameter = new ContextParameter();
        }

        /// <summary>
        /// 取得数据库访问对象。
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// 取得日志对象。
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// 取得或设置当前用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 取得上下文动态参数。
        /// </summary>
        public dynamic Parameter { get; }

        /// <summary>
        /// 创建一个默认数据库和日志的上下文对象实例。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <returns>上下文对象实例。</returns>
        public static Context Create(string userName = null)
        {
            var database = new Database();
            var logger = new FileLogger();
            return new Context(database, logger, userName);
        }
    }

    class ContextParameter : DynamicObject
    {
        private readonly Dictionary<string, object> datas = new Dictionary<string, object>();

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return datas.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (datas.ContainsKey(binder.Name))
            {
                result = datas[binder.Name];
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            datas[binder.Name] = value;
            return true;
        }
    }
}
