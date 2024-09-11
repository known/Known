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
        Visible = true;
        Enabled = true;
        Closable = true;
        Children = [];
        Columns = [];
    }

    internal MenuInfo(SysModule module, bool isAdmin = true) : this()
    {
        if (isAdmin)
            module.LoadData();

        Data = module;
        Id = module.Id;
        Name = module.Name;
        Icon = module.Icon;
        Description = module.Description;
        ParentId = module.ParentId;
        Code = module.Code;
        Target = module.Target;
        Url = module.Url;
        Sort = module.Sort;
        Model = DataHelper.GetEntity(module.EntityData);
        Page = module.Page;
        Form = module.Form;
        Tools = module.Buttons;
        Actions = module.Actions;
        Columns = module.Columns;
    }

    internal MenuInfo(SysOrganization model) : this()
    {
        Id = model.Id;
        ParentId = model.ParentId;
        Code = model.Code;
        Name = model.Name;
        Data = model;
    }

    internal MenuInfo(MenuInfo model) : this()
    {
        Id = model.Id;
        ParentId = model.ParentId;
        Code = model.Code;
        Name = model.Name;
        Icon = model.Icon;
        Description = model.Description;
        Target = model.Target;
        Url = model.Url;
        Sort = model.Sort;
        Color = model.Color;
        Tools = model.Tools;
        Actions = model.Actions;
        Columns = model.Columns;
    }

    internal MenuInfo(string id, string name, string icon = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
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
    /// 取得或设置菜单目标类型（Menu/Page/Custom/IFrame）。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置菜单返回URL，适用于APP移动端应用。
    /// </summary>
    public string BackUrl { get; set; }

    /// <summary>
    /// 取得或设置菜单背景颜色，适用于APP移动端应用。
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 取得或设置菜单显示顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置菜单徽章数量，适用于APP移动端应用。。
    /// </summary>
    public int Badge { get; set; }

    /// <summary>
    /// 取得或设置菜单是否可见。
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 取得或设置菜单是否可用。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置菜单是否可关闭，适用于多标签页。
    /// </summary>
    public bool Closable { get; set; }

    /// <summary>
    /// 取得或设置菜单是否勾选，适用于角色权限配置。
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// 取得或设置上级菜单对象。
    /// </summary>
    public MenuInfo Parent { get; set; }

    /// <summary>
    /// 取得或设置子菜单对象列表。
    /// </summary>
    public List<MenuInfo> Children { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的数据对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的实体模型对象。
    /// </summary>
    public EntityInfo Model { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的页面模型对象。
    /// </summary>
    public PageInfo Page { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的表单模型对象。
    /// </summary>
    public FormInfo Form { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的工具条权限按钮列表。
    /// </summary>
    public List<string> Tools { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的表格操作列权限按钮列表。
    /// </summary>
    public List<string> Actions { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的页面表格权限栏位信息列表。
    /// </summary>
    public List<PageColumnInfo> Columns { get; set; }

    /// <summary>
    /// 取得菜单对应的路由URL。
    /// </summary>
    public string RouteUrl
    {
        get
        {
            if (Target != ModuleType.Page.ToString() && Target != ModuleType.IFrame.ToString())
                return Url;

            if (string.IsNullOrWhiteSpace(Url))
                return $"/page/{Id}";

            return Url.StartsWith("/") ? $"/page{Url}" : $"/page/{Url}";
        }
    }

    internal bool HasTool(string id) => Tools != null && Tools.Contains(id);
    internal bool HasAction(string id) => Actions != null && Actions.Contains(id);
    internal bool HasColumn(string id) => Columns != null && Columns.Exists(c => c.Id == id);

    internal List<CodeInfo> GetAllActions()
    {
        var codes = new List<CodeInfo>();
        if (Tools != null && Tools.Count > 0)
            codes.AddRange(Tools.Select(b => GetAction(this, b)));
        if (Actions != null && Actions.Count > 0)
            codes.AddRange(Actions.Select(b => GetAction(this, b)));
        return codes;
    }

    internal List<CodeInfo> GetAllColumns()
    {
        var codes = new List<CodeInfo>();
        if (Columns != null && Columns.Count > 0)
            codes.AddRange(Columns.Select(b => new CodeInfo($"c_{Id}_{b.Id}", b.Name)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, string id)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = Config.Actions.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
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
        }
    }

    internal ActionInfo(Context context, string idOrName, string icon) : this(context, idOrName)
    {
        Icon = icon;
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
    /// 取得或设置操作URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置操作样式，如：primary，danger等。
    /// </summary>
    public string Style { get; set; }

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
    public List<ActionInfo> Children { get; }

    /// <summary>
    /// 取得或设置操作单击事件方法。
    /// </summary>
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}

/// <summary>
/// 栏位信息类。
/// </summary>
public class ColumnInfo
{
    internal ColumnInfo(string id, RenderFragment template)
    {
        Id = id;
        Template = template;
    }

    internal ColumnInfo(PageColumnInfo info) => SetPageColumnInfo(info);
    internal ColumnInfo(FormFieldInfo info) => SetFormFieldInfo(info);
    internal ColumnInfo(PropertyInfo info) => SetPropertyInfo(info);

    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置栏位提示文字。
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见，默认True。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段。
    /// </summary>
    public bool IsSort { get; set; }

    /// <summary>
    /// 取得或设置栏位默认排序方法（升序/降序）。
    /// </summary>
    public string DefaultSort { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查看连接（设为True，才可在线配置表单，为False，则默认为普通查询表格）。
    /// </summary>
    public bool IsViewLink { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查询条件。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件下拉框是否显示【全部】。
    /// </summary>
    public bool IsQueryAll { get; set; }

    /// <summary>
    /// 取得或设置栏位固定列位置（left/right）。
    /// </summary>
    public string Fixed { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 取得或设置栏位对齐方式（left/center/right）。
    /// </summary>
    public string Align { get; set; }

    internal bool IsForm { get; set; }

    /// <summary>
    /// 取得或设置栏标题名称。
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置表单字段控件占位符文本。
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// 取得或设置表单字段行号，默认1。
    /// </summary>
    public int Row { get; set; } = 1;

    /// <summary>
    /// 取得或设置表单字段列号，默认1。
    /// </summary>
    public int Column { get; set; } = 1;

    /// <summary>
    /// 取得或设置表单字段跨度大小，整行跨度为24。
    /// </summary>
    public int? Span { get; set; }

    /// <summary>
    /// 取得或设置栏位字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置表单字段附件是否可多选。
    /// </summary>
    public bool MultiFile { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// 取得或设置表单字段是否为只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置表单字段代码表列表。
    /// </summary>
    public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 取得或设置表单字段呈现模板。
    /// </summary>
    public RenderFragment Template { get; set; }

    /// <summary>
    /// 取得或设置栏位关联的对象属性。
    /// </summary>
    public PropertyInfo Property { get; private set; }

    /// <summary>
    /// 取得或设置栏位备注。
    /// </summary>
    public string Note { get; set; }

    internal string GetImportRuleNote(Context context)
    {
        if (!string.IsNullOrWhiteSpace(Category))
        {
            var codes = Cache.GetCodes(Category);
            return context.Language["Import.TemplateFill"].Replace("{text}", $"{string.Join(",", codes.Select(c => c.Code))}");
        }

        return Note;
    }

    private void SetPageColumnInfo(PageColumnInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name;
        IsViewLink = info.IsViewLink;
        IsQuery = info.IsQuery;
        IsQueryAll = info.IsQueryAll;
        IsSum = info.IsSum;
        IsSort = info.IsSort;
        DefaultSort = info.DefaultSort;
        Fixed = info.Fixed;
        Width = info.Width ?? 0;
        Align = info.Align;
    }

    internal void SetFormFieldInfo(FormFieldInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name;
        Row = info.Row;
        Column = info.Column;
        Span = info.Span;
        Type = info.Type;
        MultiFile = info.MultiFile;
        ReadOnly = info.ReadOnly;
        Required = info.Required;
        Placeholder = info.Placeholder;
        Category = info.Category;
    }

    internal void SetPropertyInfo(PropertyInfo info)
    {
        if (info == null)
            return;

        Property = info;
        Id = info.Name;

        if (string.IsNullOrWhiteSpace(Name))
        {
            var name = info.GetCustomAttribute<DisplayNameAttribute>();
            if (name != null)
                Name = name.DisplayName;
        }

        var required = info.GetCustomAttribute<RequiredAttribute>();
        if (required != null)
            Required = true;

        var code = info.GetCustomAttribute<CategoryAttribute>();
        if (code != null)
            Category = code.Category;

        Type = info.GetFieldType();

        var form = info.GetCustomAttribute<FormAttribute>();
        if (form != null)
        {
            IsForm = true;
            Row = form.Row;
            Column = form.Column;
            if (!string.IsNullOrWhiteSpace(form.Type))
                Type = Utils.ConvertTo<FieldType>(form.Type);
            ReadOnly = form.ReadOnly;
            Placeholder = form.Placeholder;
        }
    }
}