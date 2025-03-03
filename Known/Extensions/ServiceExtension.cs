namespace Known.Extensions;

/// <summary>
/// 依赖注入服务扩展类。
/// </summary>
public static class ServiceExtension
{
    /// <summary>
    /// 创建依赖注入的接口实例。
    /// </summary>
    /// <typeparam name="T">接口类型。</typeparam>
    /// <param name="factory">依赖注入服务工厂实例。</param>
    /// <returns></returns>
    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory)
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        return service;
    }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <param name="factory">依赖注入服务工厂实例。</param>
    /// <param name="context">上下文对象实例。</param>
    /// <returns></returns>
    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory, Context context) where T : IService
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        service.Context = context;
        return service;
    }

    /// <summary>
    /// 添加注入程序集中 Service 特性的服务类、继承 EntityBase 类的实体类、并自动创建新数据库表。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assembly">程序集。</param>
    public static void AddServices(this IServiceCollection services, Assembly assembly)
    {
        if (assembly == null)
            return;

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(EntityBase)))
                DbConfig.Models.Add(item);

            var attr = item.GetCustomAttribute<ServiceAttribute>();
            if (attr == null)
                continue;

            services.AddServices(attr.Lifetime, item);
        }
    }

    /// <summary>
    /// 添加注入程序集中 Client 特性的客户端类。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assembly">程序集。</param>
    public static void AddClients(this IServiceCollection services, Assembly assembly)
    {
        if (assembly == null)
            return;

        foreach (var item in assembly.GetTypes())
        {
            var attr = item.GetCustomAttribute<ClientAttribute>();
            if (attr == null)
                continue;

            services.AddServices(attr.Lifetime, item);
        }
    }

    private static void AddServices(this IServiceCollection services, ServiceLifetime lifetime, Type item)
    {
        var interfaces = item.GetInterfaces().Where(s => s.Name != nameof(IService)).ToList();
        switch (lifetime)
        {
            case ServiceLifetime.Scoped:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddScoped(item);
                else
                    services.AddScoped(interfaces[0], item);
                break;
            case ServiceLifetime.Singleton:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddSingleton(item);
                else
                    services.AddSingleton(interfaces[0], item);
                break;
            case ServiceLifetime.Transient:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddTransient(item);
                else
                    services.AddTransient(interfaces[0], item);
                break;
            default:
                break;
        }
    }
}