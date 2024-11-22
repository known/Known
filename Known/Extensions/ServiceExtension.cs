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
}