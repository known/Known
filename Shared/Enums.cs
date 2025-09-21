namespace Known;

/// <summary>
/// 性别类型枚举。
/// </summary>
public enum GenderType
{
    /// <summary>
    /// 男。
    /// </summary>
    [Description("男")]
    Male,
    /// <summary>
    /// 女。
    /// </summary>
    [Description("女")]
    Female
}

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
    Custom,
    /// <summary>
    /// 隐藏字段。
    /// </summary>
    Hidden,
    /// <summary>
    /// 整型。
    /// </summary>
    Integer
}

/// <summary>
/// 菜单类型枚举。
/// </summary>
public enum MenuType
{
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 页面。
    /// </summary>
    Page,
    /// <summary>
    /// 链接。
    /// </summary>
    Link,
    /// <summary>
    /// 原型。
    /// </summary>
    [CodeIgnore]
    Prototype
}

/// <summary>
/// 模块菜单类型枚举，适用于Admin插件。
/// </summary>
public enum ModuleType
{
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 无代码表格页面。
    /// </summary>
    Page,
    /// <summary>
    /// 自定义页面。
    /// </summary>
    Custom,
    /// <summary>
    /// IFrame页面。
    /// </summary>
    IFrame,
    /// <summary>
    /// 无代码表单页面。
    /// </summary>
    [CodeIgnore]
    Form
}

/// <summary>
/// 服务生命周期枚举。
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// 作用域。
    /// </summary>
    Scoped,
    /// <summary>
    /// 单例。
    /// </summary>
    Singleton,
    /// <summary>
    /// 瞬时。
    /// </summary>
    Transient
}

/// <summary>
/// 连接打开目标位置。
/// </summary>
public enum LinkTarget
{
    /// <summary>
    /// 当前窗口。
    /// </summary>
    None,
    /// <summary>
    /// 新窗口。
    /// </summary>
    Blank,
    /// <summary>
    /// iFrame窗口。
    /// </summary>
    IFrame
}

/// <summary>
/// 默认值类型枚举。
/// </summary>
public enum DefaultValueType
{
    /// <summary>
    /// 固定值。
    /// </summary>
    [Description("固定值")] 
    Fixed,
    /// <summary>
    /// 占位符。
    /// </summary>
    [Description("占位符")] 
    Placeholder
}

/// <summary>
/// 字典类别类型枚举。
/// </summary>
public enum DicCategoryType
{
    /// <summary>
    /// 数据字典。
    /// </summary>
    [Description("数据字典")]
    Dictionary,

    /// <summary>
    /// 自定义。
    /// </summary>
    [Description("自定义")]
    Custom
}

/// <summary>
/// 密码复杂度枚举。
/// </summary>
public enum PasswordComplexity
{
    /// <summary>
    /// 不限制。
    /// </summary>
    [Description("不限制")]
    None,
    /// <summary>
    /// 低。
    /// </summary>
    [Description("低（满足长度）")]
    Low,
    /// <summary>
    /// 中（包含字母和数字）。
    /// </summary>
    [Description("中（包含大小写和数字）")]
    Middle,
    /// <summary>
    /// 高（包含字母、数字和特殊字符）。
    /// </summary>
    [Description("高（包含大小写、数字和特殊字符）")]
    High
}