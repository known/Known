namespace Known.Weixins;

public static class UIExtension
{
    public static async Task ShowWeixinQRCodeAsync(this BasePage page, QRCodeOption option)
    {
        var weixin = await page.Platform.Weixin.GetWeixinAsync();
        if (weixin == null || !weixin.IsWeixinAuth)
            return;

        var user = page.CurrentUser;
        var wxUser = await page.Platform.Weixin.GetWeixinAsync(user);
        if (wxUser == null)
        {
            //扫码场景ID：{场景ID}_{用户ID}
            var qrCodeUrl = await page.Platform.Weixin.GetQRCodeUrlAsync($"{option.SceneId}_{user.Id}");
            ShowWeixinQRCode(page, option, qrCodeUrl, user);
        }
    }

    private static void ShowWeixinQRCode(BasePage page, QRCodeOption option, string qrCodeUrl, UserInfo user)
    {
        var isManualClose = false;
        var model = new DialogModel
        {
            Title = "微信扫码关注",
            Width = 300,
            Content = b =>
            {
                var text = option.Text;
                if (string.IsNullOrWhiteSpace(text))
                    text = "请使用微信扫码关注公众号！";
                b.Div().Style("margin-bottom:10px;text-align:center;").Markup(text).Close();
                b.Image().Style("width:250px;height:250px;").Src(qrCodeUrl).Close();
            }
        };
        model.OnClosed = () => isManualClose = true;
        page.UI.ShowDialog(model);
        Task.Run(async () =>
        {
            while (true)
            {
                if (isManualClose)
                {
                    Logger.Info("[WeixinQRCode] Scanning Manual Closed!");
                    break;
                }

                var weixin = await page.Platform.Weixin.GetWeixinAsync(user);
                if (weixin != null)
                {
                    await model.CloseAsync();
                    await page.UI.Toast("关注成功！");
                    break;
                }
                Thread.Sleep(1000);
            }
        });
    }
}