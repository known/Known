namespace WebSite.Data;

public class MenuItem
{
    public MenuItem(string id, string name, string description = "")
    {
        Id = id;
        Name = name;
        Description = description;
        Children = new List<MenuItem>();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Type Type { get; set; }
    public List<MenuItem> Children { get; set; }
}

static class MenuExtension
{
    internal static void Add<T>(this List<MenuItem> items, string name, string description)
    {
        var type = typeof(T);
        items.Add(new MenuItem(type.Name.Substring(1), name, description) { Type = type });
    }
}