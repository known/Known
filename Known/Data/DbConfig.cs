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
}