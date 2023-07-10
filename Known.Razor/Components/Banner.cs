namespace Known.Razor.Components;

public class Banner : BaseComponent
{
    [Parameter] public bool IsShow { get; set; } = true;
    [Parameter] public StyleType Style { get; set; } = StyleType.Primary;
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!IsShow)
            return;

        var css = CssBuilder.Default("banner").AddClass(Style.ToString().ToLower()).Build();
        builder.Div(css, attr =>
        {
            Content?.Invoke(builder);
            builder.Icon("close fa fa-close", "", Callback(() => IsShow = false));
        });
    }
}