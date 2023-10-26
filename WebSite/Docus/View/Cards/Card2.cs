namespace WebSite.Docus.View.Cards;

class Card2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-card", attr =>
        {
            builder.Component<KCard>()
                   .Set(c => c.Head, BuildHead)
                   .Set(c => c.Body, BuildBody)
                   .Build();
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        builder.Span("这里是自定义");
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Span("Card Body");
    }
}