using WebSite.Docus.View.Carousels;

namespace WebSite.Docus.View;

class KCarousel : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "用于轮流播放一组图片"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Carousel1>("1.默认示例");
    }
}