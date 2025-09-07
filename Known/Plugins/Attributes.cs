namespace Known.Plugins;

/// <summary>
/// 插件特性类，用于标识组件是否是插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute(string name, string icon) : Attribute
{
    /// <summary>
    /// 取得插件名称。
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 取得插件菜单图标。
    /// </summary>
    public string Icon { get; } = icon;

    /// <summary>
    /// 取得或设置插件分类。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置排序。
    /// </summary>
    public int Sort {  get; set; }

    /// <summary>
    /// 取得或设置插件角色。
    /// </summary>
    public string Role { get; set; }
}

/// <summary>
/// 开发中心插件特性类，用于标识组件是否是开发中心插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class DevPluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }

/// <summary>
/// 顶部导航插件特性类，用于标识组件是否是顶部导航条插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class NavPluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }

/// <summary>
/// 页面插件特性类，用于标识组件是否是页面区块插件。
/// </summary>
public class PagePluginAttribute : PluginAttribute
{
    /// <summary>
    /// 构造函数，创建一个页面插件特性类实例。
    /// </summary>
    /// <param name="name">插件名称。</param>
    /// <param name="icon">插件菜单图标。</param>
    /// <param name="type">页面类型。</param>
    public PagePluginAttribute(string name, string icon, PagePluginType type) : base(name, icon)
    {
        Type = type;
        Category = type.ToString();
    }

    /// <summary>
    /// 取得页面插件类型。
    /// </summary>
    public PagePluginType Type { get; }
}

/// <summary>
/// 页面区块插件类型枚举。
/// </summary>
public enum PagePluginType
{
    /// <summary>
    /// 模块。
    /// </summary>
    [Description("模块")]
    Module,
    /// <summary>
    /// 表格。
    /// </summary>
    [Description("表格")]
    Table,
    /// <summary>
    /// 表单。
    /// </summary>
    [Description("表单")]
    Form,
    /// <summary>
    /// 详情。
    /// </summary>
    [Description("详情")]
    Detail,
    /// <summary>
    /// 列表。
    /// </summary>
    [Description("列表")]
    List,
    /// <summary>
    /// 图表。
    /// </summary>
    [Description("图表")]
    Chart,
    /// <summary>
    /// 模板。
    /// </summary>
    [Description("模板")]
    Template,
    /// <summary>
    /// AI。
    /// </summary>
    [Description("AI")]
    AI,
    /// <summary>
    /// 其他。
    /// </summary>
    [Description("其他")]
    Other
}