namespace Known.Razor;

public class KBanner : BaseComponent
{
    private bool isShow = true;

    [Parameter] public StyleType Style { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isShow)
            return;

        var css = CssBuilder.Default("banner").AddClass(Style.ToString().ToLower()).Build();
        builder.Div(css, attr =>
        {
            Content?.Invoke(builder);
            builder.Icon("close fa fa-close", "", Callback(() => isShow = false));
        });
    }
}