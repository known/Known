using WebSite.Docus.View.ImageBoxs;

namespace WebSite.Docus.View;

class KImageBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 显示缩略图片
- 点击缩略图可放大查看原图
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<ImageBox1>("1.默认示例");
    }
}