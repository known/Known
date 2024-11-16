namespace Known.Data;

/// <summary>
/// 数据访问配置类。
/// </summary>
public sealed class DbConfig
{
    private DbConfig() { }

    internal static Dictionary<Type, Type> TableNames { get; } = [];

    /// <summary>
    /// 映射数据库实体。
    /// </summary>
    /// <typeparam name="TFrom">框架实体类型。</typeparam>
    /// <typeparam name="TTo">具体数据库实体类型。</typeparam>
    public static void MapEntity<TFrom, TTo>()
    {
        TableNames[typeof(TFrom)] = typeof(TTo);
    }

    /// <summary>
    /// 获取数据库配置信息列表。
    /// </summary>
    /// <returns></returns>
    public static List<DatabaseInfo> GetDatabases()
    {
        return DatabaseOption.Instance.Connections.Select(c => new DatabaseInfo
        {
            Name = c.Name,
            Type = c.DatabaseType.ToString(),
            ConnectionString = GetDefaultConnectionString(c)
        }).ToList();
    }

    /// <summary>
    /// 加载保存的数据库连接配置信息。
    /// </summary>
    /// <param name="onLoad">加载数据库配置信息委托。</param>
    public static void LoadConnections(Action<List<ConnectionInfo>> onLoad)
    {
        onLoad?.Invoke(DatabaseOption.Instance.Connections);
    }

    /// <summary>
    /// 设置数据库连接配置信息。
    /// </summary>
    /// <param name="infos">数据库配置信息列表。</param>
    /// <param name="onSave">保存数据库配置信息委托。</param>
    public static void SetConnections(List<DatabaseInfo> infos, Action<List<ConnectionInfo>> onSave = null)
    {
        if (infos == null || infos.Count == 0)
            return;

        foreach (var info in infos)
        {
            var conn = DatabaseOption.Instance.GetConnection(info.Name);
            if (conn != null)
                conn.ConnectionString = info.ConnectionString;
        }

        onSave?.Invoke(DatabaseOption.Instance.Connections);
    }

    private static string GetDefaultConnectionString(Data.ConnectionInfo info)
    {
        switch (info.DatabaseType)
        {
            case DatabaseType.Access:
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password=xxx";
            case DatabaseType.SQLite:
                return "Data Source=..\\Sample.db";
            case DatabaseType.SqlServer:
                return "Data Source=localhost;Initial Catalog=Sample;User Id=xxx;Password=xxx;";
            case DatabaseType.Oracle:
                return "Data Source=localhost:1521/orcl;User Id=xxx;Password=xxx;";
            case DatabaseType.MySql:
                return "Data Source=localhost;port=3306;Initial Catalog=Sample;user id=xxx;password=xxx;Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
            case DatabaseType.PgSql:
                return "Host=localhost;Port=5432;Database=Sample;Username=xxx;Password=xxx;";
            case DatabaseType.DM:
                return "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;";
            default:
                return string.Empty;
        }
    }
}