namespace Known.Data;

/// <summary>
/// 数据库访问配置选项。
/// </summary>
public class DatabaseOption
{
    internal static DatabaseOption Instance { get; } = new();

    /// <summary>
    /// 取得数据库类型列表。
    /// </summary>
    public static List<string> Types { get; } = [];

    /// <summary>
    /// 取得系统数据库配置信息列表，注意：Default 为框架默认数据库连接配置名称，不要修改。
    /// </summary>
    public List<DatabaseInfo> Databases { get; } = [];

    /// <summary>
    /// 取得或设置系统数据库SQL监听器委托。
    /// </summary>
    public Action<CommandInfo> SqlMonitor { get; set; }

    /// <summary>
    /// 添加数据库访问提供者。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="name">配置名称。</param>
    /// <param name="type">数据库类型。</param>
    /// <param name="connString">连接字符串。</param>
    public void AddProvider<TProvider>(string name, DatabaseType type, string connString)
    {
        AddProvider<TProvider>(type);
        Databases.Add(new DatabaseInfo
        {
            Name = name,
            DatabaseType = type,
            ProviderType = typeof(TProvider),
            ConnectionString = connString
        });
    }

    /// <summary>
    /// 添加 Access 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddAccess<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.Access);
        else
            AddProvider<TProvider>(name, DatabaseType.Access, connString);
    }

    /// <summary>
    /// 添加 SQLite 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddSQLite<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.SQLite);
        else
            AddProvider<TProvider>(name, DatabaseType.SQLite, connString);
    }

    /// <summary>
    /// 添加 SqlServer 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddSqlServer<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.SqlServer);
        else
            AddProvider<TProvider>(name, DatabaseType.SqlServer, connString);
    }

    /// <summary>
    /// 添加 Oracle 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddOracle<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.Oracle);
        else
            AddProvider<TProvider>(name, DatabaseType.Oracle, connString);
    }

    /// <summary>
    /// 添加 MySql 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddMySql<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.MySql);
        else
            AddProvider<TProvider>(name, DatabaseType.MySql, connString);
    }

    /// <summary>
    /// 添加 PgSql 数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddPgSql<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.PgSql);
        else
            AddProvider<TProvider>(name, DatabaseType.PgSql, connString);
    }

    /// <summary>
    /// 添加达梦数据库连接配置。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">配置名称。</param>
    public void AddDM<TProvider>(string connString = null, string name = Database.DefaultConnName)
    {
        if (string.IsNullOrWhiteSpace(connString))
            AddProvider<TProvider>(DatabaseType.DM);
        else
            AddProvider<TProvider>(name, DatabaseType.DM, connString);
    }

    /// <summary>
    /// 获取指定配置名的数据库配置信息。
    /// </summary>
    /// <param name="name">配置名称。</param>
    /// <returns>数据库配置信息。</returns>
    public DatabaseInfo GetDatabase(string name)
    {
        if (Databases == null || Databases.Count == 0)
            return null;

        return Databases.FirstOrDefault(c => c.Name == name);
    }

    private static void AddProvider<TProvider>(DatabaseType type)
    {
        var key = type.ToString();
        if (!DbProviderFactories.GetProviderInvariantNames().Contains(key))
        {
            Types.Add(key);
            DbProviderFactories.RegisterFactory(key, typeof(TProvider));
        }
    }
}