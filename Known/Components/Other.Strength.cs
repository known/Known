namespace Known.Components;

/// <summary>
/// 强度组件。
/// </summary>
public class KStrength : BaseComponent
{
    private string ClassName => CssBuilder.Default("kui-strength").AddClass(Value).BuildClass();

    /// <summary>
    /// 取得或设置强度（weak，medium，strong）。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置提示文本。
    /// </summary>
    [Parameter] public string Tips { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Div(ClassName, () =>
        {
            builder.Div("kui-strength-bar", () =>
            {
                builder.Div("kui-strength-inner", Language[Text]);
            });
            builder.Div("kui-strength-text", Language[Tips]);
        });
    }
}