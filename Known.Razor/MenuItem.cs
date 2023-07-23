namespace Known.Razor;

public class MenuItem : MenuInfo
{
    public MenuItem()
    {
        Children = new List<MenuItem>();
    }

    public MenuItem(string code, string name, string icon = null) : this()
    {
        Code = code;
        Name = name;
        Icon = icon;
    }

    public MenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = new List<MenuItem>();
    }

    public MenuItem(string name, string icon, Action action) : this()
    {
        Name = name;
        Icon = icon;
        Action = action;
    }

    internal MenuItem(ButtonInfo info, Action action)
    {
        Icon = info.Icon;
        Name = info.Name;
        Action = action;
    }

    public Action Action { get; set; }
    public Type ComType { get; set; }
    public Dictionary<string, object> ComParameters { get; set; }
    public MenuItem Previous { get; set; }
    public MenuItem Parent { get; set; }
    public List<MenuItem> Children { get; set; }

    internal string PageId => $"{Id}-{Name}";

    public static MenuItem From(MenuInfo menu)
    {
        return new MenuItem
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