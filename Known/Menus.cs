using Known.Entities;

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

        var infos = Cache.Get<List<ActionInfo>>(Key);
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

    //public bool Is(ActionInfo info) => Id == info.Id;

    internal static void Load()
    {
        var path = Path.Combine(Config.ContentRoot, "actions.txt");
        if (!File.Exists(path))
            return;

        var infos = new List<ActionInfo>();
        var lines = File.ReadAllLines(path);
        foreach (var item in lines)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            var values = item.Split('|');
            if (values.Length < 2)
                continue;

            var info = new ActionInfo();
            if (values.Length > 0)
                info.Id = values[0].Trim();
            if (values.Length > 1)
                info.Name = values[1].Trim();
            if (values.Length > 2)
                info.Icon = values[2].Trim();
            if (values.Length > 3)
                info.Style = values[3].Trim();
            infos.Add(info);
        }
        Cache.Set(Key, infos);
    }
}

//public enum ColumnType { Text, Number, Boolean, Date, DateTime }
//public enum AlignType { Left, Center, Right }

public class ColumnInfo
{
    //public ColumnInfo()
    //{
    //    Type = ColumnType.Text;
    //    Align = AlignType.Left;
    //}

    //public ColumnInfo(string name, string id, ColumnType type = ColumnType.Text, bool isQuery = false) : this()
    //{
    //    Id = id;
    //    Name = name;
    //    Type = type;
    //    IsQuery = isQuery;
    //    SetColumnType();
    //}

    public string Id { get; set; }
    public string Name { get; set; }
    //public ColumnType Type { get; set; }
    //public AlignType Align { get; set; }
    public int Width { get; set; }
    public int Sort { get; set; }
    public bool IsQuery { get; set; }
    public bool IsAdvQuery { get; set; }
    public bool IsSum { get; set; }
    public bool IsSort { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    public bool IsFixed { get; set; }

    //public void SetColumnType(Type type)
    //{
    //    var typeName = type.FullName;
    //    if (typeName.Contains("System.Boolean"))
    //        Type = ColumnType.Boolean;
    //    else if (typeName.Contains("System.DateTime"))
    //        Type = ColumnType.Date;
    //    else if (typeName.Contains("System.Decimal") || typeName.Contains("System.Int32"))
    //        Type = ColumnType.Number;

    //    SetColumnType();
    //}

    //private void SetColumnType()
    //{
    //    if (Type == ColumnType.Boolean || Type == ColumnType.Date || Type == ColumnType.DateTime)
    //        Align = AlignType.Center;
    //    else if (Type == ColumnType.Number)
    //        Align = AlignType.Right;
    //}
}

public class MenuItem : MenuInfo
{
    public MenuItem()
    {
        Children = [];
    }

    //public MenuItem(string code, string name, string icon = null) : this()
    //{
    //    Code = code;
    //    Name = name;
    //    Icon = icon;
    //}

    public MenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = [];
    }

    //public MenuItem(string name, string icon, Action action) : this()
    //{
    //    Name = name;
    //    Icon = icon;
    //    Action = action;
    //}

    //internal MenuItem(ActionInfo info, Action action)
    //{
    //    Icon = info.Icon;
    //    Name = info.Name;
    //    Enabled = info.Enabled;
    //    Visible = info.Visible;
    //    Action = action;
    //}

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public Action Action { get; set; }
    public Type ComType { get; set; }
    public Dictionary<string, object> ComParameters { get; set; }
    public MenuItem Previous { get; set; }
    public MenuItem Parent { get; set; }
    public List<MenuItem> Children { get; set; }
    public object Data { get; set; }

    internal string PageId => $"{Id}-{Name}";

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