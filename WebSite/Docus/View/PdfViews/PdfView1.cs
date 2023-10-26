using WebSite.Data;

namespace WebSite.Docus.View.PdfViews;

class PdfView1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var stream = FileService.GetPdfStream();
        builder.Component<KPdfView>("pdfView")
               .Set(c => c.Style, "demo-pdf")
               .Set(c => c.Stream, stream)
               .Build();
    }
}