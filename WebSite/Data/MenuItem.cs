namespace WebSite.Data;

public class MenuItem
{
    public MenuItem(string id, string name, string keyword = "")
    {
        Id = id;
        Name = name;
        Keyword = keyword;
    }

    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Keyword { get; set; }
    public List<MenuItem>? Children { get; set; }
}