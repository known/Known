using WebSite.Docus.View.ImageBoxs;

namespace WebSite.Docus.View;

class KImageBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "显示图片"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<ImageBox1>("1.默认示例");
    }
}