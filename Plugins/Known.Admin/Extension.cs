namespace Known.Admin;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加Known框架后台权限管理模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">选项委托。</param>
    public static void AddKnownAdmin(this IServiceCollection services, Action<AdminOption> action = null)
    {
        var assembly = typeof(Extension).Assembly;
        Config.AddModule(assembly);
        KStyleSheet.AddStyleSheet("_content/Known.Admin/css/web.css");
    }
}