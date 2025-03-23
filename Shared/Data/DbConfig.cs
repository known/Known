namespace Known.Data;

/// <summary>
/// 数据访问配置类。
/// </summary>
public sealed class DbConfig
{
    private DbConfig() { }

    internal static Dictionary<Type, Type> TableNames { get; } = [];

    /// <summary>
    /// 取得数据模型配置信息列表，适用于EFCore配置模型。
    /// </summary>
    public static List<DbModelInfo> Models { get; } = [];

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
    public static List<ConnectionInfo> GetConnections()
    {
        return DatabaseOption.Instance.Databases.Select(d => new ConnectionInfo
        {
            Name = d.Name,
            Type = d.DatabaseType.ToString(),
            ConnectionString = d.ConnectionString
        }).ToList();
    }

    /// <summary>
    /// 加载保存的数据库连接配置信息。
    /// </summary>
    /// <param name="onLoad">加载数据库配置信息委托。</param>
    public static void LoadConnections(Action<List<DatabaseInfo>> onLoad)
    {
        onLoad?.Invoke(DatabaseOption.Instance.Databases);
    }

    /// <summary>
    /// 设置数据库连接配置信息。
    /// </summary>
    /// <param name="infos">数据库配置信息列表。</param>
    /// <param name="onSave">保存数据库配置信息委托。</param>
    public static void SetConnections(List<ConnectionInfo> infos, Action<List<DatabaseInfo>> onSave = null)
    {
        if (infos == null || infos.Count == 0)
            return;

        foreach (var info in infos)
        {
            var conn = DatabaseOption.Instance.GetDatabase(info.Name);
            if (conn != null)
                conn.ConnectionString = info.ConnectionString;
        }

        onSave?.Invoke(DatabaseOption.Instance.Databases);
    }
}