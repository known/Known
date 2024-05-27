namespace Known;

public class MenuInfo
{
    public MenuInfo()
    {
        Visible = true;
        Columns = [];
    }

    internal MenuInfo(string id, string name, string icon = null, string description = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
        Description = description;
    }

    internal MenuInfo(SysModule module, bool isAdmin = true) : this()
    {
        if (isAdmin)
            module.LoadData();

        Id = module.Id;
        Name = module.Name;
        Icon = module.Icon;
        Description = module.Description;
        ParentId = module.ParentId;
        Code = module.Code;
        Target = module.Target;
        Url = module.Url;
        Sort = module.Sort;
        Tools = module.Buttons;
        Actions = module.Actions;
        Columns = module.Columns;
    }

    public string Id { get; set; }
    public string ParentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public string Target { get; set; }
    public string Url { get; set; }
    public string BackUrl { get; set; }
    public string Color { get; set; }
    public int Sort { get; set; }
    public int Badge { get; set; }
    public bool Visible { get; set; }
    internal List<string> Tools { get; set; }
    internal List<string> Actions { get; set; }
    internal List<PageColumnInfo> Columns { get; set; }

    public string RouteUrl
    {
        get
        {
            var url = Url;
            if (string.IsNullOrWhiteSpace(url) || Target == ModuleType.IFrame.ToString())
                url = $"/page/{Id}";
            return url;
        }
    }

    internal bool HasTool(string id) => Tools != null && Tools.Contains(id);
    internal bool HasAction(string id) => Actions != null && Actions.Contains(id);
    internal bool HasColumn(string id) => Columns != null && Columns.Exists(c => c.Id == id);

    public List<CodeInfo> GetAllActions()
    {
        var codes = new List<CodeInfo>();
        if (Tools != null && Tools.Count > 0)
            codes.AddRange(Tools.Select(b => GetAction(this, b)));
        if (Actions != null && Actions.Count > 0)
            codes.AddRange(Actions.Select(b => GetAction(this, b)));
        return codes;
    }

    public List<CodeInfo> GetAllColumns()
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

public class ActionInfo
{
    public ActionInfo()
    {
        Enabled = true;
        Visible = true;
        Children = [];
    }

    public ActionInfo(string idOrName) : this(null, idOrName) { }

    private ActionInfo(Context context, string idOrName) : this()
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

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public string Style { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public List<ActionInfo> Children { get; }
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}

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

    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsVisible { get; set; } = true;

    public bool IsSum { get; set; }
    public bool IsSort { get; set; }
    public string DefaultSort { get; set; }
    public bool IsViewLink { get; set; }
    public bool IsQuery { get; set; }
    public bool IsQueryAll { get; set; }
    public string Fixed { get; set; }
    public int Width { get; set; }
    public string Align { get; set; }

    internal bool IsForm { get; set; }
    public string Category { get; set; }
    public string Placeholder { get; set; }
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public FieldType Type { get; set; }
    public bool MultiFile { get; set; }
    public bool Required { get; set; }
    public bool ReadOnly { get; set; }

    public RenderFragment Template { get; set; }
    public PropertyInfo Property { get; private set; }

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

    internal void SetPageColumnInfo(PageColumnInfo info)
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

public class MenuItem : MenuInfo
{
    public MenuItem()
    {
        Closable = true;
        Children = [];
    }

    internal MenuItem(SysModule module) : base(module)
    {
        Closable = true;
        Data = module;
        Children = [];
    }

    internal MenuItem(SysOrganization model) : this()
    {
        Id = model.Id;
        ParentId = model.ParentId;
        Code = model.Code;
        Name = model.Name;
        Data = model;
    }

    internal MenuItem(MenuInfo model) : this()
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

    internal MenuItem(string id, string name, string icon = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    public bool Enabled { get; set; } = true;
    public bool Closable { get; set; }
    public bool Checked { get; set; }
    public MenuItem Previous { get; set; }
    public MenuItem Parent { get; set; }
    public List<MenuItem> Children { get; set; }
    public object Data { get; set; }
}