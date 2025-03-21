namespace Known;

/// <summary>
/// 字段类型枚举。
/// </summary>
public enum FieldType
{
    //添加字段类型，只能在最后添加，否则会影响模块表单配置
    /// <summary>
    /// 文本框。
    /// </summary>
    Text,
    /// <summary>
    /// 多行文本框。
    /// </summary>
    TextArea,
    /// <summary>
    /// 日期选择框。
    /// </summary>
    Date,
    /// <summary>
    /// 数字选择框。
    /// </summary>
    Number,
    /// <summary>
    /// 开关。
    /// </summary>
    Switch,
    /// <summary>
    /// 复选框。
    /// </summary>
    CheckBox,
    /// <summary>
    /// 复选框列表。
    /// </summary>
    CheckList,
    /// <summary>
    /// 单选列表。
    /// </summary>
    RadioList,
    /// <summary>
    /// 选择框。
    /// </summary>
    Select,
    /// <summary>
    /// 密码框。
    /// </summary>
    Password,
    /// <summary>
    /// 附件。
    /// </summary>
    File,
    /// <summary>
    /// 日期时间选择框。
    /// </summary>
    DateTime,
    /// <summary>
    /// 自动完成。
    /// </summary>
    AutoComplete,
    /// <summary>
    /// 自定义。
    /// </summary>
    Custom
}