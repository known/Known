namespace Known;

/// <summary>
/// 管理插件配置类。
/// </summary>
public sealed class AdminConfig
{
    private AdminConfig() { }
    
    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
    internal static SystemInfo System { get; set; }

    /// <summary>
    /// 取得关于系统页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, RenderFragment> SystemTabs { get; } = [];
}