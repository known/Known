namespace Known;

public class MenuInfo
{
    public MenuInfo()
    {
        Columns = new List<ColumnInfo>();
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