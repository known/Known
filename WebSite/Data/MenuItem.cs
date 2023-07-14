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

    public string? Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; }
    public List<MenuItem> Children { get; set; }
}