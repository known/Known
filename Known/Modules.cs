namespace Known;

public class EntityInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsFlow { get; set; }
    public List<FieldInfo> Fields { get; set; } = [];
}

public enum FieldType { Text, TextArea, Date, DateTime, Number, Switch, CheckBox, CheckList, RadioList, Select, Password, File }

public class FieldInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public FieldType Type { get; set; }
    public string Length { get; set; }
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
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsViewLink { get; set; }
    public bool IsQuery { get; set; }
    public bool IsQueryAll { get; set; }
    public bool IsSum { get; set; }
    public bool IsSort { get; set; }
    public string DefaultSort { get; set; }
    public string Fixed { get; set; }
    public string Width { get; set; }
    public string Align { get; set; }
}

public class FormInfo
{
    public double? Width { get; set; }
    public bool Maximizable { get; set; }
    public bool DefaultMaximized { get; set; }
    public int? LabelSpan { get; set; }
    public int? WrapperSpan { get; set; }
    public List<FormFieldInfo> Fields { get; set; } = [];
}

public class FormFieldInfo : FieldInfo
{
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public string CategoryType { get; set; }
    public string Category { get; set; }
    public string Placeholder { get; set; }
    public bool ReadOnly { get; set; }
    public bool MultiFile { get; set; }
}