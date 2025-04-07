namespace Known.Components;

/// <summary>
/// 移动端表单组件类。
/// </summary>
public partial class AppForm
{
    private string ClassName => CssBuilder.Default("kui-app-form").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置子内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 取得或设置操作模板。
    /// </summary>
    [Parameter] public RenderFragment Action { get; set; }
}