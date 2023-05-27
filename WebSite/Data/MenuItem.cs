namespace WebSite.Data;

public class MenuItem
{
    public MenuItem(string id, string name)
    {
        Id = id;
        Name = name;
        Children = new List<MenuItem>();
    }

    public string? Id { get; set; }
    public string? Name { get; set; }
    public List<MenuItem> Children { get; set; }
}