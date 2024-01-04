using System.ComponentModel;

namespace Known;

public class EntityInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsFlow { get; set; }
    public List<FieldInfo> Fields { get; set; } = [];
}

public enum FieldType { Text, TextArea, Date, Number, Switch, CheckBox, CheckList, RadioList, Select, Password, File }

public class FieldInfo
{
    [DisplayName("ID")]
    public string Id { get; set; }
    [DisplayName("Name")]
    public string Name { get; set; }
    [DisplayName("Type")]
    public FieldType Type { get; set; }
    [DisplayName("Length")]
    public string Length { get; set; }
    [DisplayName("Required")]
    public bool Required { get; set; }
}

public class PageInfo
{
    public string Type { get; set; }
    public bool ShowPager { get; set; }
    public string FixedWidth { get; set; }
    public string FixedHeight { get; set; }
    public string[] Tools { get; set; }
    public string[] Actions { get; set; }
    public List<PageColumnInfo> Columns { get; set; } = [];
}

public class PageColumnInfo
{
    [DisplayName("ID")]
    public string Id { get; set; }
    [DisplayName("Name")]
    public string Name { get; set; }
    [DisplayName("IsViewLink")]
    public bool IsViewLink { get; set; }
    [DisplayName("IsQuery")]
    public bool IsQuery { get; set; }
    [DisplayName("IsQueryAll")]
    public bool IsQueryAll { get; set; }
    [DisplayName("IsSort")]
    public bool IsSort { get; set; }
    [DisplayName("DefaultSort")]
    public string DefaultSort { get; set; }
    [DisplayName("Fixed")]
    public string Fixed { get; set; }
    [DisplayName("Width")]
    public string Width { get; set; }
}

public class FormInfo
{
    public int? LabelSpan { get; set; }
    public int? WrapperSpan { get; set; }
    public List<FormFieldInfo> Fields { get; set; } = [];
}

public class FormFieldInfo : FieldInfo
{
    [DisplayName("Row")]
    public int Row { get; set; } = 1;
    [DisplayName("Column")]
    public int Column { get; set; } = 1;
    [DisplayName("Category")]
    public string Category { get; set; }
    [DisplayName("Placeholder")]
    public string Placeholder { get; set; }
    [DisplayName("ReadOnly")]
    public bool ReadOnly { get; set; }
    [DisplayName("MultiFile")]
    public bool MultiFile { get; set; }
}