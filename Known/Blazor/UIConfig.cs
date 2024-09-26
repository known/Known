namespace Known.Blazor;

/// <summary>
/// 框架UI全局配置类。
/// </summary>
public class UIConfig
{
    private UIConfig() { }

    /// <summary>
    /// 取得或设置页面高度自适应JS脚本。
    /// </summary>
    public static string FillHeightScript { get; set; }

    /// <summary>
    /// 取得或设置系统字体大小下拉项列表。
    /// </summary>
    public static List<ActionInfo> Sizes { get; set; } = [];

    /// <summary>
    /// 取得或设置系统图标字典。
    /// </summary>
    public static Dictionary<string, List<string>> Icons { get; set; } = [];

    /// <summary>
    /// 取得错误页面配置字典。
    /// </summary>
    public static Dictionary<string, ErrorConfigInfo> Errors { get; } = [];

    /// <summary>
    /// 取得或设置自定义自动表格页面委托。
    /// </summary>
    public static Action<RenderTreeBuilder, TableModel<Dictionary<string, object>>> AutoTablePage { get; set; }

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