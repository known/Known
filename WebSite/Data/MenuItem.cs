namespace WebSite.Data;

public class MenuItem
{
    public MenuItem(string name, string url = "", string keyword = "")
    {
        Name = name;
        Url = url;
        Keyword = keyword;
    }

    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Keyword { get; set; }
    public List<MenuItem>? Children { get; set; }
}