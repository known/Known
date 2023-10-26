namespace WebSite.Docus.View.Cards;

class Card1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-card", attr =>
        {
            builder.Component<KCard>()
                   .Set(c => c.Icon, "fa fa-list")
                   .Set(c => c.Name, "Card1")
                   .Set(c => c.Body, BuildBody)
                   .Build();
        });
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Span("Card Body");
    }
}