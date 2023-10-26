namespace Known.Razor;

public class KMenuItem : MenuInfo
{
    public KMenuItem()
    {
        Children = new List<KMenuItem>();
    }

    public KMenuItem(string code, string name, string icon = null) : this()
    {
        Code = code;
        Name = name;
        Icon = icon;
    }

    public KMenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = new List<KMenuItem>();
    }

    public KMenuItem(string name, string icon, Action action) : this()
    {
        Name = name;
        Icon = icon;
        Action = action;
    }

    internal KMenuItem(ButtonInfo info, Action action)
    {
        Icon = info.Icon;
        Name = info.Name;
        Enabled = info.Enabled;
        Visible = info.Visible;
        Action = action;
    }

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public Action Action { get; set; }
    public Type ComType { get; set; }
    public Dictionary<string, object> ComParameters { get; set; }
    public KMenuItem Previous { get; set; }
    public KMenuItem Parent { get; set; }
    public List<KMenuItem> Children { get; set; }

    internal string PageId => $"{Id}-{Name}";

    public static KMenuItem From(MenuInfo menu)
    {
        return new KMenuItem
        {
            Id = menu.Id,
            ParentId = menu.ParentId,
            Code = menu.Code,
            Name = menu.Name,
            Icon = menu.Icon,
            Description = menu.Description,
            Target = menu.Target,
            Sort = menu.Sort,
            Color = menu.Color,
            Badge = menu.Badge,
            Buttons = menu.Buttons,
            Actions = menu.Actions,
            Columns = menu.Columns
        };
    }
}