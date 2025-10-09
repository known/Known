namespace Known.Blazor;

/// <summary>
/// 呈现类型枚举。
/// </summary>
public enum RenderType
{
    /// <summary>
    /// 自动模式。
    /// </summary>
    Auto,
    /// <summary>
    /// SSR模式。
    /// </summary>
    Server
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
    [Description("查看")]
    View,
    /// <summary>
    /// 提交工作流。
    /// </summary>
    [Description("提交")]
    Submit,
    /// <summary>
    /// 审核工作流。
    /// </summary>
    [Description("审核")]
    Verify
}