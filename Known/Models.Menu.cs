using AntDesign;

namespace Known;

/// <summary>
/// 菜单信息类。
/// </summary>
public partial class MenuInfo
{
    /// <summary>
    /// 取得或设置上级菜单对象。
    /// </summary>
    [JsonIgnore]
    public MenuInfo Parent { get; set; }

    /// <summary>
    /// 取得或设置子菜单对象列表。
    /// </summary>
    [JsonIgnore]
    public List<MenuInfo> Children { get; set; } = [];

    /// <summary>
    /// 取得或设置菜单关联的数据对象。
    /// </summary>
    [JsonIgnore]
    public object Data { get; set; }

    /// <summary>
    /// 取得菜单关联的页面组件类型。
    /// </summary>
    [JsonIgnore]
    public Type PageType { get; internal set; }

    /// <summary>
    /// 取得菜单对应的路由URL。
    /// </summary>
    [JsonIgnore]
    public string RouteUrl
    {
        get
        {
            if (Type == nameof(MenuType.Prototype))
                return $"/page/{Id}";

            if (Type == nameof(MenuType.Page))
                return !string.IsNullOrWhiteSpace(Url) ? GetPageUrl(Url) : $"/page/{Id}";

            if (Target != nameof(ModuleType.Page) && Target != nameof(ModuleType.IFrame))
                return Url;

            if (string.IsNullOrWhiteSpace(Url) || Target == nameof(ModuleType.IFrame))
                return $"/page/{Id}";

            // 无代码页面有自定义URL，例如：/page/自定义
            return GetPageUrl(Url);
        }
    }

    /// <summary>
    /// 获取菜单的字符串表示。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name}({RouteUrl})";
    }

    /// <summary>
    /// 添加子菜单。
    /// </summary>
    /// <param name="menu">菜单信息。</param>
    public void AddChild(MenuInfo menu)
    {
        menu.Parent = this;
        Children.Add(menu);
    }

    /// <summary>
    /// 添加多个子菜单。
    /// </summary>
    /// <param name="menus">菜单列表。</param>
    public void AddChildren(List<MenuInfo> menus)
    {
        if (menus == null || menus.Count == 0)
            return;

        foreach (var menu in menus)
        {
            AddChild(menu);
        }
    }

    internal bool HasRoute(string url, RouteData route)
    {
        if (Target == Constants.Route)
            return Id == route.PageType.FullName || PageType == route.PageType;

        return RouteUrl == url;
    }

    internal bool HasUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(url))
            return false;

        if (Url == url)
            return true;

        var route1 = Url.Split('?')[0];
        var route2 = url.Split('?')[0];
        return route1 == route2;
    }

    private static string GetPageUrl(string url)
    {
        if (Config.RouteTypes.ContainsKey(url))
            return url;

        return url.StartsWith("/") ? $"/page{url}" : $"/page/{url}";
    }
}

/// <summary>
/// 操作信息类。
/// </summary>
public partial class ActionInfo
{
    /// <summary>
    /// 取得或设置操作单击事件方法。
    /// </summary>
    [JsonIgnore]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// 转换成按钮信息。
    /// </summary>
    /// <returns></returns>
    public ButtonInfo ToButton()
    {
        return new ButtonInfo
        {
            Id = Id,
            Name = Name,
            Icon = Icon,
            Style = Style,
            Position = Position?.Split(',')
        };
    }

    internal ButtonType ToType()
    {
        return ButtonExtension.GetButtonType(Style);
    }

    internal bool IsDanger()
    {
        return ButtonExtension.GetButtonDanger(Style);
    }
}