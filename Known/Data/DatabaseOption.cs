namespace Known.Data;

/// <summary>
/// 数据库访问配置选项。
/// </summary>
public class DatabaseOption
{
    internal static DatabaseOption Instance { get; } = new();

    /// <summary>
    /// 取得系统数据库连接信息列表，注意：Default 为框架默认数据库连接名称，不要修改。
    /// </summary>
    public List<ConnectionInfo> Connections { get; } = [];

    /// <summary>
    /// 取得或设置系统数据库SQL监听器委托。
    /// </summary>
    public Action<CommandInfo> SqlMonitor { get; set; }

    /// <summary>
    /// 添加数据库访问提供者。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="name">连接名称。</param>
    /// <param name="type">数据库类型。</param>
    /// <param name="connString">连接字符串。</param>
    public void AddProvider<TProvider>(string name, DatabaseType type, string connString)
    {
        var key = type.ToString();
        if (!DbProviderFactories.GetProviderInvariantNames().Contains(key))
            DbProviderFactories.RegisterFactory(key, typeof(TProvider));

        Connections.Add(new ConnectionInfo
        {
            Name = name,
            DatabaseType = type,
            ProviderType = typeof(TProvider),
            ConnectionString = connString
        });
    }

    /// <summary>
    /// 添加 Access 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddAccess<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.Access, connString);
    }

    /// <summary>
    /// 添加 SQLite 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddSQLite<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.SQLite, connString);
    }

    /// <summary>
    /// 添加 SqlServer 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddSqlServer<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.SqlServer, connString);
    }

    /// <summary>
    /// 添加 Oracle 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddOracle<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.Oracle, connString);
    }

    /// <summary>
    /// 添加 MySql 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddMySql<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.MySql, connString);
    }

    /// <summary>
    /// 添加 PgSql 数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddPgSql<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.PgSql, connString);
    }

    /// <summary>
    /// 添加达梦数据库连接。
    /// </summary>
    /// <typeparam name="TProvider">提供者类型。</typeparam>
    /// <param name="connString">连接字符串。</param>
    /// <param name="name">连接名称。</param>
    public void AddDM<TProvider>(string connString, string name = Database.DefaultConnName)
    {
        AddProvider<TProvider>(name, DatabaseType.DM, connString);
    }

    /// <summary>
    /// 获取指定连接名的数据库连接信息。
    /// </summary>
    /// <param name="name">连接名称。</param>
    /// <returns>数据库连接信息。</returns>
    public ConnectionInfo GetConnection(string name)
    {
        if (Connections == null || Connections.Count == 0)
            return null;

        return Connections.FirstOrDefault(c => c.Name == name);
    }
}