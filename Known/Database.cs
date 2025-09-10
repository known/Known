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
    /// 创建一个事务数据库访问实例。
    /// </summary>
    /// <returns></returns>
    protected virtual Database CreateDatabase()
    {
        var database = new Database(loggerFactory);
        var info = DatabaseOption.Instance.GetDatabase(ConnectionName);
        if (info != null)
            database.SetDatabase(info);
        else
            database.SetDatabase(ConnectionName, DatabaseType, ConnectionString);
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

        if (info != null)
        {
            var message = info.ToString();
            Logger.Information(LogTarget.BackEnd, User, message);
            Console.WriteLine(message);
        }
        Logger.Exception(LogTarget.BackEnd, User, ex);
    }
}