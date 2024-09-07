namespace Known;

/// <summary>
/// 代码表特性类，用于标识常量类作为数据字典代码。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

/// <summary>
/// 表单特性类，用于编码方式设置实体类属性作为动态表单字段。
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute() : Attribute
{
    /// <summary>
    /// 取得或设置字段所属行号，默认1。
    /// </summary>
    public int Row { get; set; } = 1;

    /// <summary>
    /// 取得或设置字段所属列号，默认1。
    /// </summary>
    public int Column { get; set; } = 1;

    /// <summary>
    /// 取得或设置字段组件类型，默认Text。
    /// </summary>
    public string Type { get; set; } = FieldType.Text.ToString();

    /// <summary>
    /// 取得或设置字段是否是只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置字段控件占位符字符串。
    /// </summary>
    public string Placeholder { get; set; }
}