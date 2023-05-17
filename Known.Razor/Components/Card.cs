namespace Known.Razor.Components;

public class Card : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment HeadTemplate { get; set; }
    [Parameter] public RenderFragment BodyTemplate { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div($"card {Style}", attr =>
        {
            BuildHead(builder);
            BuildBody(builder);
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        builder.Div("card-head", attr =>
        {
            if (HeadTemplate != null)
                builder.Fragment(HeadTemplate);
            else
                builder.IconName(Icon, Title);
        });
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Div("card-body", attr =>
        {
            builder.Fragment(BodyTemplate);
        });
    }
}