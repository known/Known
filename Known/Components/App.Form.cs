namespace Known.Components;

/// <summary>
/// 移动端表单组件类。
/// </summary>
public class AppForm : BaseComponent
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

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Class(ClassName).Child(() =>
        {
            builder.Fragment(ChildContent);
            if (Action != null)
            {
                builder.Div("kui-app-form-action", () => builder.Fragment(Action));
            }
        });
    }
}