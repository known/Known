namespace Known;

/// <summary>
/// 插件类型枚举。
/// </summary>
public enum PluginType
{
    /// <summary>
    /// 开发。
    /// </summary>
    Dev,
    /// <summary>
    /// 导航。
    /// </summary>
    Navbar,
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 页面。
    /// </summary>
    Page
}

/// <summary>
/// 插件特性类，用于标识组件是否是插件。
/// </summary>
/// <param name="type">插件类型。</param>
/// <param name="name">插件名称。</param>
[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute(PluginType type, string name) : Attribute
{
    /// <summary>
    /// 取得插件组件ID。
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// 取得插件类型。
    /// </summary>
    public PluginType Type { get; } = type;

    /// <summary>
    /// 取得插件名称。
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 取得或设置插件菜单图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置插件分类。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得插件组件类型。
    /// </summary>
    public Type Component { get; internal set; }

    /// <summary>
    /// 取得插件组件URL。
    /// </summary>
    public string Url { get; internal set; }
}