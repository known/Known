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
    /// 取得或设置菜单关联的组件类型。
    /// </summary>
    public Type Page { get; set; }
}

/// <summary>
/// 动作特性类，用于标识方法是否需要角色权限控制。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute
{
    /// <summary>
    /// 取得或设置操作图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作名称。
    /// </summary>
    public string Name { get; set; }
}