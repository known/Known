namespace Known;

/// <summary>
/// 菜单信息类。
/// </summary>
public partial class MenuInfo
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
    [Required]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置菜单图标。
    /// </summary>
    [Required]
    [DisplayName("图标")]
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
    [DisplayName("目标")]
    public string Target { get; set; } = nameof(LinkTarget.None);

    /// <summary>
    /// 取得或设置菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [DisplayName("顺序")]
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
    /// 取得或设置是否是代码生成的模块。
    /// </summary>
    public bool IsCode { get; set; }

    /// <summary>
    /// 取得或设置布局信息。
    /// </summary>
    public LayoutInfo Layout { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public List<PluginInfo> Plugins { get; set; } = [];

    /// <summary>
    /// 取得或设置菜单关联的数据对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 将关联的数据对象转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    /// <summary>
    /// 克隆菜单信息。
    /// </summary>
    /// <param name="isData">是否设置Data为SysModule。</param>
    /// <returns></returns>
    public MenuInfo Clone(bool isData = false)
    {
        var info = new MenuInfo
        {
            Id = Id,
            ParentId = ParentId,
            Code = Code,
            Name = Name,
            Icon = Icon,
            Description = Description,
            Type = Type,
            Target = Target,
            Url = Url,
            Sort = Sort,
            Visible = Visible,
            Enabled = Enabled,
            CanEdit = CanEdit,
            IsCode = IsCode,
            Badge = Badge,
            Layout = Layout?.Clone(),
            Plugins = Plugins?.Select(d => d.Clone()).ToList(),
            Color = Color
        };
        if (isData)
            info.Data = Data ?? GetModule();
        return info;
    }

    private SysModule GetModule()
    {
        return new SysModule
        {
            Id = Id,
            ParentId = ParentId,
            Code = Code,
            Name = Name,
            Icon = Icon,
            Description = Description,
            Type = Type,
            Target = Target,
            Url = Url,
            Sort = Sort,
            Enabled = Enabled,
            IsCode = IsCode,
            Layout = Layout?.Clone(),
            Plugins = Plugins?.Select(d => d.Clone()).ToList()
        };
    }
}

/// <summary>
/// 操作信息类。
/// </summary>
public partial class ActionInfo
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

    /// <summary>
    /// 构造函数，创建一个操作信息类的实例。
    /// </summary>
    /// <param name="idOrName">操作按钮ID或名称。</param>
    public ActionInfo(string idOrName) : this()
    {
        Id = idOrName;
        Name = idOrName;

        var info = Config.Actions?.FirstOrDefault(a => a.Id == idOrName || a.Name == idOrName);
        if (info != null)
        {
            Id = info.Id;
            Name = info.Name;
            Icon = info.Icon;
            Style = info.Style;
            Title = info.Title;
            Group = info.Group;
            Visible = info.Visible;
            Tabs = info.Tabs;
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
    /// 取得或设置操作按钮分组。
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// 取得或设置操作位置，如：Toolbar，Action。
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 取得或设置操作是否可用。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置操作是否可见。
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 取得或设置操作按钮所属选项卡集合。
    /// </summary>
    public string[] Tabs { get; set; }

    /// <summary>
    /// 取得或设置徽章数量。
    /// </summary>
    [JsonIgnore]
    public int Badge { get; set; }

    /// <summary>
    /// 取得或设置子操作列表。
    /// </summary>
    [JsonIgnore]
    public List<ActionInfo> Children { get; } = [];

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