namespace Known.Blazor;

/// <summary>
/// 框架UI全局配置类。
/// </summary>
public class UIConfig
{
    private UIConfig() { }

    /// <summary>
    /// 取得或设置系统字体大小下拉项列表。
    /// </summary>
    public static List<ActionInfo> Sizes { get; set; } = [];

    /// <summary>
    /// 取得或设置页面底部内容组件。
    /// </summary>
    public static RenderFragment Footer { get; set; }

    /// <summary>
    /// 取得或设置系统图标字典。
    /// </summary>
    public static Dictionary<string, List<string>> Icons { get; set; } = [];

    /// <summary>
    /// 取得错误页面配置字典。
    /// </summary>
    public static Dictionary<string, ErrorConfigInfo> Errors { get; } = [];

    /// <summary>
    /// 取得关于系统页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, RenderFragment> SystemTabs { get; } = [];

    /// <summary>
    /// 取得用户中心页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, RenderFragment> UserTabs { get; } = [];

    /// <summary>
    /// 取得开发中心页面自定义标签字典。
    /// </summary>
    public static Dictionary<string, RenderFragment> DevelopTabs { get; } = [];

    /// <summary>
    /// 取得开发中心模块表单自定义标签字典。
    /// </summary>
    public static Dictionary<string, Action<RenderTreeBuilder, ModuleInfo>> ModuleFormTabs { get; } = [];

    /// <summary>
    /// 取得或设置自定义自动表格页面委托。
    /// </summary>
    public static Action<RenderTreeBuilder, TableModel<Dictionary<string, object>>> AutoTablePage { get; set; }

    /// <summary>
    /// 取得或设置显示导入窗体委托。
    /// </summary>
    public static Action<RenderTreeBuilder, ImportInfo> ImportForm { get; set; }

    /// <summary>
    /// 取得忽略URL鉴权的路由列表。
    /// </summary>
    public static List<string> IgnoreRoutes { get; } = [];

    /// <summary>
    /// 取得或设置页面标签颜色委托。
    /// </summary>
    public static Func<string, string> TagColor { get; set; }

    internal static List<MenuInfo> Menus { get; } = [];

    internal static void SetMenu(MenuInfo info)
    {
        if (!Menus.Exists(m => m.Url == info.Url))
            Menus.Add(info);
    }
}

/// <summary>
/// 错误页面配置信息类。
/// </summary>
public class ErrorConfigInfo
{
    /// <summary>
    /// 取得或设置错误代码描述文本。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置错误代码自定义模板。
    /// </summary>
    public RenderFragment Template { get; set; }
}