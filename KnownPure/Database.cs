namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 创建数据库访问实例。
    /// </summary>
    /// <param name="name">数据库连接名。</param>
    /// <returns>数据库访问实例。</returns>
    /// <exception cref="SystemException">数据库访问实现类不支持。</exception>
    public static Database Create(string name = DefaultConnName)
    {
        var database = new Database();
        database.SetDatabase(name);
        return database;
    }

    /// <summary>
    /// 创建一个事务数据库访问实例。
    /// </summary>
    /// <returns></returns>
    protected virtual Database CreateDatabase()
    {
        var database = new Database();
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

        if (info != null)
        {
            Console.WriteLine(info.ToString());
            Logger.Information(LogTarget.BackEnd, User, info.ToString());
        }
        Console.WriteLine(ex);
        Logger.Error(LogTarget.BackEnd, User, ex.ToString());
    }
}