namespace Known.Data;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加Known框架数据访问提供者。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">选项委托。</param>
    public static void AddKnownData(this IServiceCollection services, Action<DataOption> action = null)
    {
        var assembly = typeof(Extension).Assembly;
        Config.AddModule(assembly);
    }
}