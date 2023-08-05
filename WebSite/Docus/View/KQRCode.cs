using WebSite.Docus.View.QRCodes;

namespace WebSite.Docus.View;

class KQRCode : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于jquery.qrcode实现
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<QRCode1>("1.默认示例");
    }
}