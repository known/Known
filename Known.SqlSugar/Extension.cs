namespace Known.SqlSugar;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加支持 SqlSugar 数据访问。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">SqlSugar 配置。</param>
    public static void AddKnownSqlSugar(this IServiceCollection services, Action<ConnectionConfig> action)
    {
        Database.RepositoryType = typeof(SqlSugarRepository);
        Database.Register(typeof(SqlSugarDatabase));

        var config = new ConnectionConfig();
        action?.Invoke(config);

        config.ConfigureExternalServices.EntityService = (p, c) =>
        {
            //设置实体主键
            if (p.Name == nameof(EntityBase.Id))
                c.IsPrimarykey = true;
            //忽略实体虚拟属性
            if (p.GetMethod != null && p.GetMethod.IsVirtual)
                c.IsIgnore = true;
        };
        SqlSugarFactory.Config = config;
    }
}