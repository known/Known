namespace Known;

/// <summary>
/// 菜单特性类，用于标识页面组件是否是模块菜单。
/// </summary>
/// <param name="parent">上级菜单。</param>
/// <param name="name">菜单名称。</param>
/// <param name="icon">菜单图标。</param>
/// <param name="sort">菜单排序。</param>
[AttributeUsage(AttributeTargets.Class)]
public class MenuAttribute(string parent, string name, string icon, int sort) : Attribute
{
    /// <summary>
    /// 取得或设置上级菜单。
    /// </summary>
    public string Parent { get; set; } = parent;

    /// <summary>
    /// 取得或设置菜单名称。
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 取得或设置菜单图标。
    /// </summary>
    public string Icon { get; set; } = icon;

    /// <summary>
    /// 取得或设置菜单排序。
    /// </summary>
    public int Sort { get; set; } = sort;

    /// <summary>
    /// 取得或设置菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置菜单角色。
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的组件类型。
    /// </summary>
    public Type Page { get; set; }
}

/// <summary>
/// 移动端菜单特性类，用于标识页面组件是否是移动端菜单。
/// </summary>
/// <param name="name">菜单名称。</param>
/// <param name="icon">菜单图标。</param>
/// <param name="sort">菜单排序。</param>
/// <param name="target">菜单目标位置，Tab/Menu，默认Menu。</param>
[AttributeUsage(AttributeTargets.Class)]
public class AppMenuAttribute(string name, string icon, int sort, string target = "Menu") : MenuAttribute("", name, icon, sort)
{
    /// <summary>
    /// 取得或设置菜单目标位置，Tab/Menu，默认Menu。
    /// </summary>
    public string Target { get; set; } = target;

    /// <summary>
    /// 取得或设置首页菜单图标背景颜色。
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 取得或设置返回页面URL。
    /// </summary>
    public string BackUrl { get; set; }
}

/// <summary>
/// 动作特性类，用于标识方法是否需要角色权限控制。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute
{
    /// <summary>
    /// 取得或设置操作按钮图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作按钮名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置操作按钮提示信息。
    /// </summary>
    public string Title { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
class LanguageAttribute(string code, string icon, bool isDefault = false, bool enabled = false) : Attribute
{
    public string Code { get; } = code;
    public string Icon { get; } = icon;
    public bool Default { get; } = isDefault;
    public bool Enabled { get; } = enabled;
}