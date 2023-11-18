using System.Reflection;
using Known.Entities;
using Known.Razor;

namespace Known;

public class MenuInfo
{
    public MenuInfo()
    {
        Columns = [];
    }

    public MenuInfo(string id, string name, string icon = null, string description = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
        Description = description;
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
    public int Badge { get; set; }
    public List<string> Buttons { get; set; }
    public List<string> Actions { get; set; }
    public List<ColumnInfo> Columns { get; set; }

    public List<CodeInfo> GetActionCodes()
    {
        var actions = ActionInfo.Actions;
        var codes = new List<CodeInfo>();
        if (Buttons != null && Buttons.Count > 0)
            codes.AddRange(Buttons.Select(b => GetButton(this, b, actions)));
        if (Actions != null && Actions.Count > 0)
            codes.AddRange(Actions.Select(b => GetButton(this, b, actions)));
        return codes;
    }

    private static CodeInfo GetButton(MenuInfo menu, string id, List<ActionInfo> buttons)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = buttons.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
    }

    public List<CodeInfo> GetColumnCodes()
    {
        var codes = new List<CodeInfo>();
        if (Columns != null && Columns.Count > 0)
            codes.AddRange(Columns.Select(b => new CodeInfo($"c_{Id}_{b.Id}", b.Name)));
        return codes;
    }
}

public class ActionInfo
{
    private const string Key = "Key_ActionInfo";

    public ActionInfo()
    {
        Enabled = true;
        Visible = true;
        Children = [];
    }

    internal ActionInfo(string idOrName)
    {
        Id = idOrName;
        Name = idOrName;

        var infos = Actions;
        var info = infos?.FirstOrDefault(a => a.Id == idOrName || a.Name == idOrName);
        if (info != null)
        {
            Id = info.Id;
            Name = info.Name;
            Icon = info.Icon;
            Style = info.Style;
        }
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Style { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public List<ActionInfo> Children { get; }

    internal static List<ActionInfo> Actions => Cache.Get<List<ActionInfo>>(Key);

    internal static void Load()
    {
        var content = Utils.GetResource(typeof(ActionInfo).Assembly, "actions");
        var lines = content.Split(Environment.NewLine);
        var infos = new List<ActionInfo>();
        AddActions(infos, lines);

        var path = Path.Combine(Config.App.ContentRoot, "actions.txt");
        if (File.Exists(path))
        {
            var lines1 = File.ReadAllLines(path);
            AddActions(infos, lines1);
        }

        Cache.Set(Key, infos);
    }

    private static void AddActions(List<ActionInfo> infos, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        foreach (var item in lines)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            var values = item.Split('|');
            if (values.Length < 2)
                continue;

            var id = values[0].Trim();
            var info = infos.FirstOrDefault(i => i.Id == id);
            if (info == null)
            {
                info = new ActionInfo { Id = id };
                infos.Add(info);
            }
            if (values.Length > 1)
                info.Name = values[1].Trim();
            if (values.Length > 2)
                info.Icon = values[2].Trim();
            if (values.Length > 3)
                info.Style = values[3].Trim();
        }
    }
}

public class ColumnInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    //public int Width { get; set; }
    //public int Sort { get; set; }
    //public bool IsQuery { get; set; }
    //public bool IsAdvQuery { get; set; }
    //public bool IsSum { get; set; }
    //public bool IsSort { get; set; } = true;
    //public bool IsVisible { get; set; } = true;
    //public bool IsFixed { get; set; }
}

public class MenuItem : MenuInfo
{
    public MenuItem()
    {
        Children = [];
    }

    internal MenuItem(string id, string name, string icon = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    public MenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = [];
    }

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
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

    internal static MenuItem From(MenuInfo model)
    {
        return new MenuItem
        {
            Id = model.Id,
            ParentId = model.ParentId,
            Code = model.Code,
            Name = model.Name,
            Icon = model.Icon,
            Description = model.Description,
            Target = model.Target,
            Sort = model.Sort,
            Color = model.Color,
            Badge = model.Badge,
            Buttons = model.Buttons,
            Actions = model.Actions,
            Columns = model.Columns
        };
    }

	internal static MenuItem From(SysModule model)
	{
		return new MenuItem
		{
			Id = model.Id,
			ParentId = model.ParentId,
			Code = model.Code,
			Name = model.Name,
			Icon = model.Icon,
			Description = model.Description,
			Target = model.Target,
			Sort = model.Sort,
			Buttons = model.Buttons,
			Actions = model.Actions,
			Columns = model.Columns,
            Data = model
		};
	}

    internal static MenuItem From(SysOrganization model)
    {
        return new MenuItem
        {
            Id = model.Id,
            ParentId = model.ParentId,
            Code = model.Code,
            Name = model.Name,
            Data = model
        };
    }
}