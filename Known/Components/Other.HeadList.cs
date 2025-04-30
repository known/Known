namespace Known.Components;

/// <summary>
/// 表头表体上下布局表单组件类。
/// </summary>
public class KHeadList : BaseComponent
{
    /// <summary>
    /// 取得或设置列表标题。
    /// </summary>
    [Parameter] public string ListTitle { get; set; }

    /// <summary>
    /// 取得或设置表头模板。
    /// </summary>
    [Parameter] public RenderFragment Head { get; set; }

    /// <summary>
    /// 取得或设置表体模板。
    /// </summary>
    [Parameter] public RenderFragment List { get; set; }

    /// <summary>
    /// 取得或设置工具栏模板。
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div(Class, () =>
        {
            builder.Fragment(Head);
            builder.Div("kui-toolbar", () =>
            {
                builder.FormTitle(Language[ListTitle]);
                builder.Div("ant-toolbar", () => builder.Fragment(Toolbar));
            });
            builder.Fragment(List);
        });
    }
}