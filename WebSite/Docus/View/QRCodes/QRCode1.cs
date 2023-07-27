namespace WebSite.Docus.View.QRCodes;

class QRCode1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认样式
        builder.QRCode("qrcode1", new { Text = "1234567890" });
        //自定义样式
        builder.QRCode("qrcode2", new
        {
            Text = "1234567890",
            Width = 180,
            Height = 180,
            Background = "#f1f1f1",
            Foreground = "#4188c8"
        });
    }
}