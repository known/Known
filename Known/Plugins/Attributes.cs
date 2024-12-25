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
}

/// <summary>
/// 开发插件特性类，用于标识组件是否是开发中心插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class DevPluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }

/// <summary>
/// 导航插件特性类，用于标识组件是否是顶部导航条插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class NavPluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }

/// <summary>
/// 菜单插件特性类，用于标识组件是否是左侧菜单插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class MenuPluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }

/// <summary>
/// 页面插件特性类，用于标识组件是否是页面区块插件。
/// </summary>
/// <param name="name">插件名称。</param>
/// <param name="icon">插件菜单图标。</param>
public class PagePluginAttribute(string name, string icon) : PluginAttribute(name, icon) { }