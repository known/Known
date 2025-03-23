namespace Known.Data;

public partial class Database
{
    private readonly ILoggerFactory loggerFactory;

    /// <summary>
    /// 构造函数，创建一个数据库访问类对象。
    /// </summary>
    /// <param name="loggerFactory">日志工厂。</param>
    public Database(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
    }

    /// <summary>
    /// 创建数据库访问实例。
    /// </summary>
    /// <param name="name">数据库连接名。</param>
    /// <returns>数据库访问实例。</returns>
    /// <exception cref="SystemException">数据库访问实现类不支持。</exception>
    public static Database Create(string name = DefaultConnName)
    {
        var scope = Config.ServiceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<Database>();
        database.SetDatabase(name);
        return database;
    }

    /// <summary>
    /// 创建一个事务数据库访问实例。
    /// </summary>
    /// <returns></returns>
    protected virtual Database CreateDatabase()
    {
        var database = new Database(loggerFactory);
        database.SetDatabase(ConnectionName);
        database.Context = Context;
        database.User = User;
        return database;
    }

    /// <summary>
    /// 处理操作异常。
    /// </summary>
    /// <param name="info">命令信息。</param>
    /// <param name="ex">异常对象。</param>
    protected void HandException(CommandInfo info, Exception ex)
    {
        if (!EnableLog)
            return;

        var logger = loggerFactory.CreateLogger<Database>();
        if (info != null)
        {
            logger.Info(info);
            Logger.Information(LogTarget.BackEnd, User, info.ToString());
        }
        logger.Error(ex);
        Logger.Error(LogTarget.BackEnd, User, ex.ToString());
    }
}