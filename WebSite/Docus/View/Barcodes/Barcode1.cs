namespace WebSite.Docus.View.Barcodes;

class Barcode1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认样式
        builder.Barcode("barcode1", "1234567890");
        //自定义样式
        builder.Barcode("barcode2", "1234567890", new
        {
            Height = 50,
            DisplayValue = false,
            Background = "#f1f1f1",
            LineColor = "#4188c8"
        });
    }
}