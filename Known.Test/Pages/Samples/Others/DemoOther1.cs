namespace Known.Test.Pages.Samples.Others;

/// <summary>
/// 输入类
/// </summary>
class DemoOther1 : BaseComponent
{
    //Button、Select、Text、TextArea、CheckBox、Switch、Hidden、Input、Password、
    //Captcha、Date、DateRange、Number、CheckList、RadioList、Picker、Upload、SearchBox

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.BuildDemo("搜索框", () => builder.Component<SearchBox>().Build());
        builder.BuildDemo("验证码", () => builder.Component<Captcha>().Build());
        BuildBarcode(builder);
        BuildQRCode(builder);
    }

    private static void BuildBarcode(RenderTreeBuilder builder)
    {
        builder.BuildDemo("条形码", () =>
        {
            builder.Barcode("barcode1", "1234567890");
            builder.Barcode("barcode2", "1234567890", new
            {
                Height = 50,
                DisplayValue = false,
                Background = "#f1f1f1",
                LineColor = "#4188c8"
            });
        });
    }

    private static void BuildQRCode(RenderTreeBuilder builder)
    {
        builder.BuildDemo("二维码", () =>
        {
            builder.QRCode("qrcode1", new { Text = "1234567890" });
            builder.QRCode("qrcode2", new
            {
                Text = "1234567890",
                Width = 180,
                Height = 180,
                Background = "#f1f1f1",
                Foreground = "#4188c8"
            });
        });
    }
}