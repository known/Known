namespace Known.SqlSugar;

public static class Extension
{
    public static void AddKnownSqlSugar(this IServiceCollection services, Action<ConnectionConfig> action)
    {
        Platform.RepositoryType = typeof(SqlSugarRepository);
        Platform.RegisterDatabase(typeof(SqlSugarDatabase));

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