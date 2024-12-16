namespace Known.Data;

/// <summary>
/// 数据库访问类。
/// </summary>
public partial class Database : IDisposable
{
    internal const string DefaultConnName = "Default";

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

    private string connName;
    private IDbConnection conn;
    private IDbTransaction trans;
    private string TransId { get; set; }
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
    /// 取得或设置数据库类型。
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    /// 取得或设置当前操作用户信息。
    /// </summary>
    public UserInfo User { get; set; }

    /// <summary>
    /// 取得当前操作用户账号。
    /// </summary>
    public string UserName => User?.UserName;

    /// <summary>
    /// 取得数据库上下文对象，适用于EFCore。
    /// </summary>
    public virtual object DbContext { get; }

    /// <summary>
    /// 取得或设置是否开启错误日志，默认是。
    /// </summary>
    public bool EnableLog { get; set; } = true;

    private DbProvider provider;
    internal DbProvider Provider
    {
        get
        {
            provider ??= DbProvider.Create(DatabaseType);
            return provider;
        }
    }

    /// <summary>
    /// 设置数据库连接。
    /// </summary>
    /// <param name="connName">连接名称。</param>
    public virtual void SetDatabase(string connName)
    {
        var setting = DatabaseOption.Instance.GetConnection(connName);
        if (setting == null)
            return;

        this.connName = connName;
        DatabaseType = setting.DatabaseType;
        ConnectionString = setting.ConnectionString;
        provider = DbProvider.Create(DatabaseType);

        var factory = DbProviderFactories.GetFactory(setting.DatabaseType.ToString());
        conn = factory.CreateConnection();
        conn.ConnectionString = setting.ConnectionString;
    }

    /// <summary>
    /// 异步打开数据库。
    /// </summary>
    /// <returns></returns>
    public virtual Task OpenAsync()
    {
        if (conn != null && conn.State != ConnectionState.Open)
            conn.Open();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步关闭数据库。
    /// </summary>
    /// <returns></returns>
    public virtual Task CloseAsync()
    {
        if (conn != null && conn.State != ConnectionState.Closed)
            conn.Close();

        conn.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取日期字段转换SQL语句，如Oracle的to_date函数。
    /// </summary>
    /// <param name="name">字段名。</param>
    /// <param name="withTime">是否带时间，默认是。</param>
    /// <returns>日期字段转换SQL语句。</returns>
    public virtual string GetDateSql(string name, bool withTime = true) => Provider?.GetDateSql(name, withTime);

    /// <summary>
    /// 释放数据库访问对象。
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// 检查实体对象状态（新增/修改）。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    protected virtual void CheckEntity<T>(T entity) where T : EntityBase, new() { }

    /// <summary>
    /// 创建一个事务数据库访问实例。
    /// </summary>
    /// <returns></returns>
    protected virtual Database CreateDatabase()
    {
        var database = new Database(loggerFactory);
        database.SetDatabase(connName);
        database.Context = Context;
        database.User = User;
        return database;
    }

    /// <summary>
    /// 获取数据库操作命令。
    /// </summary>
    /// <param name="info">命令信息对象。</param>
    /// <returns>数据库操作命令。</returns>
    /// <exception cref="ArgumentException">有事务，则连接不能为空</exception>
    protected virtual IDbCommand GetDbCommandAsync(CommandInfo info)
    {
        info.IsClose = false;
        var cmd = conn.CreateCommand();

        if (trans != null)
        {
            if (trans.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof(trans));

            cmd.Transaction = trans;
        }
        else
        {
            if (conn.State != ConnectionState.Open)
            {
                info.IsClose = true;
                conn.Open();
            }
        }

        return cmd;
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
        logger.Info(info);
        logger.Error(ex);
    }

    /// <summary>
    /// 释放数据库访问对象。
    /// </summary>
    /// <param name="isDisposing">是否释放。</param>
    protected virtual void Dispose(bool isDisposing)
    {
        trans?.Dispose();
        trans = null;

        if (conn == null)
            return;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
    }

    /// <summary>
    /// 格式化数据库表或字段名，例如加 []、"" 等。
    /// </summary>
    /// <param name="name">表或字段名。</param>
    /// <returns>格式化的表或字段名。</returns>
    public string FormatName(string name) => Provider?.FormatName(name);

    private Task<IDbCommand> PrepareCommandAsync(CommandInfo info)
    {
        DatabaseOption.Instance.SqlMonitor?.Invoke(info);

        var cmd = GetDbCommandAsync(info);
        cmd.CommandText = info.Text;
        if (info.Params != null && info.Params.Count > 0)
        {
            cmd.Parameters.Clear();
            foreach (var item in info.Params)
            {
                var pName = $"{info.Prefix}{item.Key}";
                if (info.Text.Contains(pName))
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = pName;
                    p.Value = GetParameterValue(item, info.IsSave);
                    cmd.Parameters.Add(p);
                }
            }
        }

        return Task.FromResult(cmd);
    }

    private object GetParameterValue(KeyValuePair<string, object> item, bool isTrim)
    {
        if (item.Value == null)
            return DBNull.Value;

        if (item.Value is bool boolean)
            return boolean.ToString();

        if (item.Value is Enum)
            return item.Value.ToString();

        if (item.Value is DateTime time)
            return DatabaseType == DatabaseType.Access ? time.ToString() : time;

        if (item.Value is string value)
            return isTrim ? TrimValue(value) : value;

        if (item.Value is JsonElement element)
        {
            var valueString = element.ToString();
            return isTrim ? TrimValue(valueString) : valueString;
        }

        return item.Value;
    }

    private static string TrimValue(string value) => value.Trim('\r', '\n').Trim();
}