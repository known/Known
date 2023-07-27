using WebSite.Docus.View.Barcodes;

namespace WebSite.Docus.View;

class KBarcode : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "基于JsBarcode实现"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Barcode1>("1.默认示例");
    }
}