using System.Web;
using Known.Services;

namespace Known.Winxins;

class WeixinService(Context context) : ServiceBase(context)
{
    internal const string KeyWeixin = "WeixinInfo";

    public Task<WeixinInfo> GetWeixinAsync()
    {
        return SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
    }

    public async Task<Result> SaveWeixinAsync(WeixinInfo model)
    {
        await SystemService.SaveConfigAsync(Database, KeyWeixin, model);
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<string> GetAuthorizeUrlAsync(string state)
    {
        var info = await SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
        if (info == null || !info.IsWeixinAuth)
            return string.Empty;

        var redirectUri = HttpUtility.UrlEncode(info.RedirectUri);
        state = HttpUtility.UrlEncode(state);
        return WeixinApi.GetAuthorizeUrl(info.AppId, redirectUri, state);
    }
}