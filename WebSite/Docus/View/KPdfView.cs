using WebSite.Docus.View.PdfViews;

namespace WebSite.Docus.View;

class KPdfView : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于pdfobject.js实现
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<PdfView1>("1.默认示例");
        builder.BuildDemo<PdfView2>("2.弹窗示例");
    }
}