namespace Known;

/// <summary>
/// 菜单信息类。
/// </summary>
public class MenuInfo
{
    /// <summary>
    /// 构造函数，创建一个菜单信息类的实例。
    /// </summary>
    public MenuInfo()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置菜单ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级菜单ID。
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置菜单代码。
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置菜单名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置菜单图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置菜单描述信息。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置菜单类型（Menu/Page/Link）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置URL目标类型（None/Blank/IFrame）；
    /// 或者菜单目标类型（Menu/Page/Custom/IFrame）。
    /// </summary>
    public string Target { get; set; } = nameof(LinkTarget.None);

    /// <summary>
    /// 取得或设置菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置菜单角色。
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置菜单返回URL，适用于APP移动端应用。
    /// </summary>
    public string BackUrl { get; set; }

    /// <summary>
    /// 取得或设置菜单背景颜色，适用于APP移动端应用。
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 取得或设置菜单徽章数量。
    /// </summary>
    public int Badge { get; set; }

    /// <summary>
    /// 取得或设置菜单是否可见，默认可见。
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 取得或设置菜单是否可用，默认可用。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置菜单是否可关闭，适用于多标签页，默认可关闭。
    /// </summary>
    public bool Closable { get; set; } = true;

    /// <summary>
    /// 取得或设置菜单是否勾选，适用于角色权限配置。
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// 取得或设置菜单是否可以编辑，默认可编辑。
    /// </summary>
    public bool CanEdit { get; set; } = true;

    /// <summary>
    /// 取得或设置布局信息。
    /// </summary>
    public LayoutInfo Layout { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public List<PluginInfo> Plugins { get; set; } = [];

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

    private AutoPageInfo tablePage;
    internal AutoPageInfo TablePage
    {
        get
        {
            tablePage ??= this.GetAutoPageParameter();
            if (tablePage == null)
            {
                tablePage = new AutoPageInfo();
                Plugins.AddPlugin(tablePage);
            }
            return tablePage;
        }
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
public class ActionInfo
{
    /// <summary>
    /// 构造函数，创建一个操作信息类的实例。
    /// </summary>
    public ActionInfo()
    {
        Enabled = true;
        Visible = true;
        Children = [];
    }

    internal ActionInfo(string idOrName) : this(null, idOrName) { }

    /// <summary>
    /// 构造函数，创建一个操作信息类的实例。
    /// </summary>
    /// <param name="context">系统上下文对象。</param>
    /// <param name="idOrName">操作按钮ID或名称。</param>
    public ActionInfo(Context context, string idOrName) : this()
    {
        Id = idOrName;
        Name = idOrName;

        var infos = Config.Actions;
        var info = infos?.FirstOrDefault(a => a.Id == idOrName || a.Name == idOrName);
        if (info != null)
        {
            Id = info.Id;
            Name = context?.Language[$"Button.{info.Id}"] ?? info.Name;
            Icon = info.Icon;
            Style = info.Style;
            Title = info.Title;
        }
    }

    /// <summary>
    /// 取得或设置操作ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置操作名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置操作图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作跳转的URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置操作样式，如：primary，danger，default，dashed，link，text等。
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// 取得或设置按钮提示信息。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置操作位置，如：Toolbar，Action。
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 取得或设置徽章数量。
    /// </summary>
    public int Badge { get; set; }

    /// <summary>
    /// 取得或设置操作是否可用。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置操作是否可见。
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 取得或设置子操作列表。
    /// </summary>
    public List<ActionInfo> Children { get; } = [];

    /// <summary>
    /// 取得或设置操作单击事件方法。
    /// </summary>
    [JsonIgnore]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    internal bool Danger => Style == "danger";

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
}