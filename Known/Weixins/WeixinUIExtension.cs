namespace Known.Weixins;

/// <summary>
/// 微信UI扩展类。
/// </summary>
public static class WeixinUIExtension
{
    /// <summary>
    /// 异步显示微信二维码。
    /// </summary>
    /// <param name="page">页面组件。</param>
    /// <param name="option">二维码选项。</param>
    /// <returns></returns>
    public static async Task ShowWeixinQRCodeAsync(this BasePage page, QRCodeOption option)
    {
        var service = await page.CreateServiceAsync<IWeixinService>();
        var qrCodeUrl = await service.GetQRCodeUrlAsync(option.SceneId);
        if (string.IsNullOrWhiteSpace(qrCodeUrl))
            return;

        var user = page.CurrentUser;
        ShowWeixinQRCode(page, service, option, qrCodeUrl, user);
    }

    private static void ShowWeixinQRCode(BasePage page, IWeixinService service, QRCodeOption option, string qrCodeUrl, UserInfo user)
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
                b.Div().Style("margin-bottom:10px;text-align:center;").Markup(text);
                b.Image().Style("width:250px;height:250px;").Src(qrCodeUrl).Close();
            }
        };
        model.OnClosed = () => isManualClose = true;
        page.UI.ShowDialog(model);
        _ = Task.Run(async () =>
        {
            while (true)
            {
                if (isManualClose)
                {
                    Logger.Information(LogTarget.FrontEnd, user, "[WeixinQRCode] Scanning Manual Closed!");
                    break;
                }

                var weixin = await service.GetWeixinByUserIdAsync(user.Id);
                if (weixin != null)
                {
                    _ = model.CloseAsync();
                    page.UI.Success("关注成功！");
                    break;
                }
                Thread.Sleep(1000);
            }
        });
    }
}