namespace Known;

public class EntityInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<FieldInfo> Fields { get; set; }
}

public class FieldInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Length { get; set; }
    public bool Required { get; set; }
}

public class PageInfo
{
    public string Type { get; set; }
    public List<string> Tools { get; set; }
    public List<string> Actions { get; set; }
}

public class FormInfo
{

}