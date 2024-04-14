using System.Web;
using Known.Entities;
using Known.Services;

namespace Known.Weixins;

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

        var redirectUri = HttpUtility.UrlEncode(WeixinApi.RedirectUri);
        state = HttpUtility.UrlEncode(state);
        return WeixinApi.GetAuthorizeUrl(redirectUri, state);
    }

    public async Task<Result> AuthorizeAsync(string token, string code)
    {
        var http = new HttpClient();
        var authToken = await http.GetAuthorizeTokenAsync(code);
        if (authToken == null || string.IsNullOrWhiteSpace(authToken.AccessToken))
            return Result.Error("AccessToken is null.");

        var user = await http.GetUserInfoAsync(authToken.AccessToken, authToken.OpenId);
        if (user == null)
            return Result.Error("WeixinUser is null.");

        var db = new Database();
        db.User = AuthService.GetUserByToken(token);
        var weixin = await WeixinRepository.GetWeixinByOpenIdAsync(db, user.OpenId);
        if (weixin == null)
        {
            user.MPAppId = WeixinApi.AppId;
            user.UserId = token;
            await db.InsertAsync(user);
        }
        return Result.Success("Authorize OK!");
    }

    public async Task<UserInfo> CheckWeixinAsync(UserInfo user)
    {
        var weixin = await WeixinRepository.GetWeixinByUserIdAsync(Database, user.Token);
        if (weixin == null)
            return null;

        weixin.UserId = user.Id;
        await Database.SaveAsync(weixin);
        user.OpenId = weixin.OpenId;
        return user;
    }

    public Task<SysWeixin> GetWeixinAsync(Database db, SysUser user)
    {
        return WeixinRepository.GetWeixinByUserIdAsync(db, user.Id);
    }

    public Task<Result> SendTemplateMessageAsync(TemplateInfo info)
    {
        var http = new HttpClient();
        return http.SendTemplateMessageAsync(info);
    }
}