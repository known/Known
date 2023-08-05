using WebSite.Data;

namespace WebSite.Docus.View.PdfViews;

class PdfView2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("显示PDF", Callback(OnShowPDF), StyleType.Primary);
    }

    private void OnShowPDF()
    {
        var stream = FileService.GetPdfStream();
        UI.ShowPdf("查看PDF", 600, 400, stream);
    }
}