namespace Known.Razor.Components;

public class Card : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public RenderFragment Head { get; set; }
    [Parameter] public RenderFragment Body { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("card").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            BuildHead(builder);
            BuildBody(builder);
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Name) && Head == null)
            return;

        builder.Div("card-head", attr =>
        {
            attr.AddRandomColor("border-bottom-color");
            if (Head != null)
                builder.Fragment(Head);
            else
                builder.IconName(Icon, Name);
        });
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Div("card-body", attr => builder.Fragment(Body));
    }
}