using System.ComponentModel;
using System.Reflection;
using Known.Blazor;
using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Known;

public class MenuInfo
{
    public MenuInfo()
    {
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
        Sort = module.Sort;
        Buttons = module.Buttons;
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
    public string Color { get; set; }
    public int Sort { get; set; }
    internal List<string> Buttons { get; set; }
    internal List<string> Actions { get; set; }
    internal List<PageColumnInfo> Columns { get; set; }

    public List<CodeInfo> GetAllActions()
    {
        var codes = new List<CodeInfo>();
        if (Buttons != null && Buttons.Count > 0)
            codes.AddRange(Buttons.Select(b => GetAction(this, b)));
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

    internal ActionInfo(string idOrName) : this()
    {
        Id = idOrName;
        Name = idOrName;

        var infos = Config.Actions;
        var info = infos?.FirstOrDefault(a => a.Id == idOrName || a.Name == idOrName);
        if (info != null)
        {
            Id = info.Id;
            Name = info.Name;
            Icon = info.Icon;
            Style = info.Style;
        }
    }

    internal ActionInfo(string idOrName, string icon) : this(idOrName)
    {
        Icon = icon;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Style { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public List<ActionInfo> Children { get; }
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}

public class ColumnInfo
{
    internal PropertyInfo Property;

    public ColumnInfo() { }
    internal ColumnInfo(PageColumnInfo info) => SetPageColumnInfo(info);
    internal ColumnInfo(FormFieldInfo info) => SetFormFieldInfo(info);

    internal ColumnInfo(PropertyInfo property)
    {
        Property = property;
        Id = Property.Name;
        Name = Property.DisplayName();

        var form = Property.GetCustomAttribute<FormAttribute>();
        if (form != null)
        {
            IsForm = true;
            Row = form.Row;
            Column = form.Column;
            Type = Utils.ConvertTo<FieldType>(form.Type);
            ReadOnly = form.ReadOnly;
            Placeholder = form.Placeholder;
            Required = Property.IsRequired();
        }

        var code = Property.GetCustomAttribute<CategoryAttribute>();
        if (code != null)
        {
            Category = code.Category;
        }
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsVisible { get; set; } = true;

    public bool IsSort { get; set; } = true;
    public string DefaultSort { get; set; }
    public bool IsViewLink { get; set; }
    public bool IsQuery { get; set; }
    public bool IsQueryAll { get; set; }
    public string Fixed { get; set; }
    public string Width { get; set; }

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
    public PropertyInfo GetProperty() => Property;

    internal void SetPageColumnInfo(PageColumnInfo info)
    {
        Id = info.Id;
        Name = info.Name;
        IsViewLink = info.IsViewLink;
        IsQuery = info.IsQuery;
        IsQueryAll = info.IsQueryAll;
        IsSort = info.IsSort;
        DefaultSort = info.DefaultSort;
        Fixed = info.Fixed;
        Width = info.Width;
    }

    internal void SetFormFieldInfo(FormFieldInfo info)
    {
        Id = info.Id;
        Name = info.Name;
        Row = info.Row;
        Column = info.Column;
        Type = info.Type;
        MultiFile = info.MultiFile;
        ReadOnly = info.ReadOnly;
        Required = info.Required;
        Placeholder = info.Placeholder;
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
        Sort = model.Sort;
        Color = model.Color;
        Buttons = model.Buttons;
        Actions = model.Actions;
        Columns = model.Columns;
    }

    internal MenuItem(string id, string name, string icon = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    public MenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Closable = true;
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = [];
    }

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public bool Closable { get; set; }
    public bool Checked { get; set; }
    public int Badge { get; set; }
    public Type ComType { get; set; }
    public Dictionary<string, object> ComParameters { get; set; }
    public MenuItem Previous { get; set; }
    public MenuItem Parent { get; set; }
    public List<MenuItem> Children { get; set; }
    public object Data { get; set; }

    private PageAttribute page;
    public PageAttribute Page
    {
        get
        {
            page ??= ComType?.GetCustomAttribute<PageAttribute>();
            return page;
        }
    }
}