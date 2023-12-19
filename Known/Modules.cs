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
    
    [Grid, DisplayName("类型")]
    public string Type { get; set; }
    
    [Grid, DisplayName("长度")]
    public string Length { get; set; }

    [Grid, DisplayName("必填")]
    public bool Required { get; set; }
}

public class PageInfo
{
    public string Type { get; set; }
    public List<string> Tools { get; set; }
    public List<string> Actions { get; set; }
    public List<PageColumnInfo> Columns { get; set; } = [];
}

public class PageColumnInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DefaultSort { get; set; }
    public bool IsViewLink { get; set; }
    public bool IsQuery { get; set; }
    public bool IsQueryAll { get; set; }
}

public class FormInfo
{
    public List<FieldInfo> Fields { get; set; } = [];
}

public class FormFieldInfo : FieldInfo
{
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public string Category { get; set; }
    public string Placeholder { get; set; }
    public bool ReadOnly { get; set; }
    public bool MultiFile { get; set; }
}