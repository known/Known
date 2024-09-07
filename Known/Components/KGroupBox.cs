namespace Known.Components;

/// <summary>
/// 分组框组件类。
/// </summary>
public class KGroupBox : BaseComponent
{
    /// <summary>
    /// 取得或设置分组框标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置分组框子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现分组框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-group-box", () =>
        {
            builder.Label().Class("legend").Text(Title).Close();
            builder.Div("body", () =>
            {
                ChildContent?.Invoke(builder);
            });
        });
    }
}