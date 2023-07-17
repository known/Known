using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Known.Test.Pages.Samples.Homes;

class DemoHome3 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBarcode(builder);
        BuildQRCode(builder);
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
                       DisplayValue = false,
                       Background = "#f1f1f1",
                       LineColor = "#4188c8"
                   })
                   .Build();
        });
    }

    private static void BuildQRCode(RenderTreeBuilder builder)
    {
        builder.BuildDemo("二维码", () =>
        {
            builder.Component<QRCode>()
                   .Set(c => c.Id, "qrcode1")
                   .Set(c => c.Option, new { Text = "1234567890" })
                   .Build();

            builder.Component<QRCode>()
                   .Set(c => c.Id, "qrcode2")
                   .Set(c => c.Option, new
                   {
                       Text = "1234567890",
                       Width = 180,
                       Height = 180,
                       Background = "#f1f1f1",
                       Foreground = "#4188c8"
                   })
                   .Build();
        });
    }
}