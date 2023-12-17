using System.ComponentModel;

namespace Known;

public class EntityInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsFlow { get; set; }
    public List<FieldInfo> Fields { get; set; } = [];
}

public class FieldInfo
{
    [Grid, DisplayName("ID")]
    public string Id { get; set; }
    [Grid, DisplayName("名称")]
    public string Name { get; set; }
    [Grid, DisplayName("数据类型")]
    public string Type { get; set; }
    [Grid, DisplayName("数据长度")]
    public string Length { get; set; }
    [Grid, DisplayName("必填")]
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