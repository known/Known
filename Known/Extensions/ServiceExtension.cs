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
        SetServiceContext(service, context);
        return service;
    }

    // 设置注入对象构造函数参数为IService的对象上下文属性。
    internal static void SetServiceContext(this object service, Context context)
    {
        if (service == null)
            return;

        var type = service.GetType();
        var properties = ServiceProperties.GetOrAdd(type, t => t.GetServiceProperties());
        foreach (var prop in properties)
        {
            if (prop.GetValue(service) is IService subService && subService.Context != context)
            {
                SetServiceContext(subService, context);
            }
        }

        var fields = ServiceFields.GetOrAdd(type, t => t.GetServiceFields());
        foreach (var field in fields)
        {
            if (field.GetValue(service) is IService subService && subService.Context != context)
            {
                SetServiceContext(subService, context);
            }
        }
    }

    internal static void AddServices(this IServiceCollection services, ServiceLifetime lifetime, Type type)
    {
        var interfaces = type.GetInterfaces().Where(s => s.Name != nameof(IService)).ToList();
        switch (lifetime)
        {
            case ServiceLifetime.Scoped:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddScoped(type);
                else
                    services.AddScoped(interfaces[0], type);
                break;
            case ServiceLifetime.Singleton:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddSingleton(type);
                else
                    services.AddSingleton(interfaces[0], type);
                break;
            case ServiceLifetime.Transient:
                if (interfaces == null || interfaces.Count == 0)
                    services.AddTransient(type);
                else
                    services.AddTransient(interfaces[0], type);
                break;
            default:
                break;
        }
    }

    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> ServiceProperties = new();
    private static readonly ConcurrentDictionary<Type, List<System.Reflection.FieldInfo>> ServiceFields = new();
    private static void SetServiceContext(IService service, Context context)
    {
        if (service == null)
            return;

        service.Context = context;
        service.SetServiceContext(context);
    }

    private static List<PropertyInfo> GetServiceProperties(this Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   .Where(p => typeof(IService).IsAssignableFrom(p.PropertyType) && p.CanRead)
                   .ToList();
    }

    private static List<System.Reflection.FieldInfo> GetServiceFields(this Type type)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   .Where(f => typeof(IService).IsAssignableFrom(f.FieldType))
                   .ToList();
    }
}