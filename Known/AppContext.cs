using Known.Data;
using Known.Log;

namespace Known
{
    /// <summary>
    /// 应用程序上下文类型。
    /// </summary>
    public class AppContext
    {
        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        public AppContext(ILogger logger) : this(null, logger) { }

        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="database">数据库访问对象。</param>
        /// <param name="logger">日志对象。</param>
        public AppContext(Database database, ILogger logger) : this(database, logger, null) { }

        /// <summary>
        /// 初始化一个应用程序上下文类型实例。
        /// </summary>
        /// <param name="database">数据库访问对象。</param>
        /// <param name="logger">日志对象。</param>
        /// <param name="userName">当前用户名。</param>
        public AppContext(Database database, ILogger logger, string userName)
        {
            if (database != null)
            {
                database.UserName = userName;
            }

            Database = database;
            Logger = logger;
            UserName = userName;
            Parameter = new DynamicParameter();
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
        /// 取得上下文动态参数。
        /// </summary>
        public dynamic Parameter { get; }

        /// <summary>
        /// 取得或设置当前用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 取得或设置文件上传路径。
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        /// 取得或设置导出模板路径。
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 创建一个默认数据库和日志的上下文对象实例。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <returns>上下文对象实例。</returns>
        public static AppContext Create(string userName = null)
        {
            var database = new Database();
            var logger = new FileLogger();
            return new AppContext(database, logger, userName);
        }
    }
}
