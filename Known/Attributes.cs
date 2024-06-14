namespace Known;

[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute() : Attribute
{
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public string Type { get; set; } = FieldType.Text.ToString();
    public bool ReadOnly { get; set; }
    public string Placeholder { get; set; }
}