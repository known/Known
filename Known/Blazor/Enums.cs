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

/// <summary>
/// 数据表单打开方式枚举。
/// </summary>
public enum FormOpenType
{
    /// <summary>
    /// 未设置。
    /// </summary>
    [CodeIgnore]
    None,
    /// <summary>
    /// 模态对话框。
    /// </summary>
    [Description("模态框")]
    Modal,
    /// <summary>
    /// 抽屉。
    /// </summary>
    [Description("抽屉")]
    Drawer,
    /// <summary>
    /// 导航连接。
    /// </summary>
    [CodeIgnore]
    Url
}