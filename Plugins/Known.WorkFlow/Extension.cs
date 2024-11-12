namespace Known.WorkFlow;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    private static readonly FlowOption option = new();

    /// <summary>
    /// 添加Known框架简易工作流模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项委托。</param>
    public static void AddKnownFlow(this IServiceCollection services, Action<FlowOption> action = null)
    {
        action?.Invoke(option);
        var assembly = typeof(Extension).Assembly;
        services.AddScoped<IFlowService, FlowService>();
        Config.AddModule(assembly);
    }
}