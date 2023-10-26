namespace WebSite.Docus.View.Carousels;

class Carousel1 : BaseComponent
{
    private readonly string[] images = new string[]
    {
        "/img/demo/sunbin.jpg",
        "/img/demo/zhuangzhou.jpg",
        "/img/demo/jialuo.jpg"
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-carousel", attr =>
        {
            builder.Component<KCarousel>()
                   .Set(c => c.Interval, 3000)
                   .Set(c => c.ShowSnk, true)
                   .Set(c => c.Images, images)
                   .Build();
        });
    }
}