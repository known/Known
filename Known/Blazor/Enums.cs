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
    /// 空白。
    /// </summary>
    None,
    /// <summary>
    /// 列布局。
    /// </summary>
    Column,
    /// <summary>
    /// 行布局。
    /// </summary>
    Row
}

/// <summary>
/// 模块菜单类型枚举。
/// </summary>
public enum ModuleType
{
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 菜单页面。
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