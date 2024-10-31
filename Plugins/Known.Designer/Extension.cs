namespace Known.Designer;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加Known无代码设计器和代码生成器。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">选项委托。</param>
    public static void AddKnownDesigner(this IServiceCollection services, Action<DesignerOption> action = null)
    {
        var assembly = typeof(Extension).Assembly;
        Config.AddModule(assembly);
        UIConfig.ModuleForm = UIHelper.BuildModuleForm;

        services.AddSingleton<ICodeGenerator, CodeGenerator>();
    }
}