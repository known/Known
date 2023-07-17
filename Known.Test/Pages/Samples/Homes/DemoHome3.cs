namespace Known.Test.Pages.Samples.Homes;

class DemoHome3 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBarcode(builder);
        
    }

    private static void BuildBarcode(RenderTreeBuilder builder)
    {
        builder.BuildDemo("条形码", () =>
        {
            builder.Component<Barcode>()
                   .Set(c => c.Id, "barcode1")
                   .Set(c => c.Value, "1234567890")
                   .Build();

            builder.Component<Barcode>()
                   .Set(c => c.Id, "barcode2")
                   .Set(c => c.Value, "1234567890")
                   .Set(c => c.Option, new
                   {
                       Height = 50,
                       DisplayValue = false
                   })
                   .Build();
        });
    }
}