namespace Known.Components;

/// <summary>
/// 表单字段项目组件类。
/// </summary>
public partial class FieldItem
{
    private string ClassName => CssBuilder.Default("ant-form-item ant-form-item-row ant-row").AddClass(Class).AddClass("kui-inline", Inline).BuildClass();
    private string LabelClass => Required ? "ant-form-item-required" : "";

    /// <summary>
    /// 取得或设置样式类。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置样式。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// 取得或设置标题模板。
    /// </summary>
    [Parameter] public RenderFragment LabelTemplate { get; set; }

    /// <summary>
    /// 取得或设置提示文字。
    /// </summary>
    [Parameter] public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置是否必填。
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// 取得或设置多个元素是否一行内显示。
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// 取得或设置子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}