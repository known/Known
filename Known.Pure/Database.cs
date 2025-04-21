namespace Known.Data;

public partial class Database
{
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
            Logger.Information(LogTarget.BackEnd, User, info.ToString());
        Logger.Exception(LogTarget.BackEnd, User, ex);
    }
}