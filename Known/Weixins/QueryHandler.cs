using Known.Blazor;

namespace Known.Weixins;

class QueryHandler(IndexPage page)
{
    private IndexPage Page { get; } = page;

    private IUIService UI => Page.UI;
    private Language Language => Page.Language;
    private PlatformService Platform => Page.Platform;
    private WeixinService Service => new(Page.Context);

    public async Task<bool> HandleAsync()
    {
        if (!string.IsNullOrWhiteSpace(Page.Token) && !string.IsNullOrWhiteSpace(Page.Code))
            return await WeixinPageAuthAsync(Page.Token, Page.Code);

        if (!string.IsNullOrWhiteSpace(Page.GZHId) && !string.IsNullOrWhiteSpace(Page.OpenId))
            return await WeixinSubscribeAsync(Page.GZHId, Page.OpenId);

        return false;
    }

    private async Task<bool> WeixinPageAuthAsync(string token, string code)
    {
        var result = await Platform.Weixin.AuthorizeAsync(token, code);
        var message = result.IsValid
                    ? Language.Success(Language.Authorize)
                    : Language.Failed(Language.Authorize);
        Logger.Info(result.Message);
        UI.Language = Language;
        UI.Alert(message, () =>
        {
            Page.Navigation.NavigateTo("/", true);
            return Task.CompletedTask;
        });
        return true;
    }

    private async Task<bool> WeixinSubscribeAsync(string gzhId, string openId)
    {
        if (gzhId != WeixinApi.GZHId)
        {
            UI.Alert("");
        }
        else
        {
            var weixin = await Service.GetWeixinAsync(openId);
            UI.Alert(openId);
        }
        return true;
    }
}