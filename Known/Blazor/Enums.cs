namespace Known.Blazor;

/// <summary>
/// 样式类型枚举。
/// </summary>
public enum StyleType
{
    /// <summary>
    /// 成功。
    /// </summary>
    Success,
    /// <summary>
    /// 信息。
    /// </summary>
    Info,
    /// <summary>
    /// 警告。
    /// </summary>
    Warning,
    /// <summary>
    /// 错误。
    /// </summary>
    Error
}

/// <summary>
/// 页面布局类型枚举。
/// </summary>
public enum PageType
{
    /// <summary>
    /// 一栏布局。
    /// </summary>
    [Description("一栏")]
    None,
    /// <summary>
    /// 两栏布局。
    /// </summary>
    [Description("两栏")]
    Column,
    /// <summary>
    /// 自定义。
    /// </summary>
    [Description("自定义")]
    Custom
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
    Link
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
    IFrame
}

/// <summary>
/// 表格勾选类型枚举。
/// </summary>
public enum TableSelectType
{
    /// <summary>
    /// 不显示。
    /// </summary>
    None,
    /// <summary>
    /// 多选框。
    /// </summary>
    Checkbox,
    /// <summary>
    /// 单选按钮。
    /// </summary>
    Radio
}

/// <summary>
/// 表格栏位自动生成模式枚举。
/// </summary>
public enum TableColumnMode
{
    /// <summary>
    /// 不自动生成。
    /// </summary>
    None,
    /// <summary>
    /// 根据实体属性生成。
    /// </summary>
    Property,
    /// <summary>
    /// 根据实体属性Column特性生成。
    /// </summary>
    Attribute
}

/// <summary>
/// 表单查看类型枚举。
/// </summary>
public enum FormViewType
{
    /// <summary>
    /// 查看详情。
    /// </summary>
    View,
    /// <summary>
    /// 提交工作流。
    /// </summary>
    Submit,
    /// <summary>
    /// 审核工作流。
    /// </summary>
    Verify
}