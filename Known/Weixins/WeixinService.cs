namespace Known.Weixins;

public class WeixinService(Context context) : ServiceBase(context)
{
    internal const string KeyWeixin = "WeixinInfo";

    #region Weixin
    public Task<WeixinInfo> GetWeixinAsync()
    {
        return SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
    }

    public async Task<Result> SaveWeixinAsync(WeixinInfo model)
    {
        await SystemService.SaveConfigAsync(Database, KeyWeixin, model);
        return Result.Success(Language.Success(Language.Save));
    }

    public Task<SysWeixin> GetWeixinAsync(string openId)
    {
        return WeixinRepository.GetWeixinByOpenIdAsync(Database, openId);
    }

    public Task<SysWeixin> GetWeixinAsync(UserInfo user)
    {
        return WeixinRepository.GetWeixinByUserIdAsync(Database, user.Id);
    }

    public Task<SysWeixin> GetWeixinAsync(Database db, SysUser user)
    {
        return WeixinRepository.GetWeixinByUserIdAsync(db, user.Id);
    }
    #endregion

    #region MP
    public async Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        using var http = new HttpClient();
        var ticket = await http.CreateTicketAsync(sceneId);
        if (ticket == null)
            return string.Empty;

        return WeixinApi.GetQRCodeUrl(ticket.Ticket);
    }

    public async Task<string> GetAuthorizeUrlAsync(string state)
    {
        var info = await SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
        if (info == null || !info.IsWeixinAuth)
            return string.Empty;

        return WeixinApi.GetAuthorizeUrl(state);
    }

    public static async Task<Result> AuthorizeAsync(string token, string code)
    {
        using var http = new HttpClient();
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

    public static async Task<string> SubscribeAsync(string openId, string userId)
    {
        try
        {
            var db = new Database();
            var weixin = await WeixinRepository.GetWeixinByOpenIdAsync(db, openId);
            weixin ??= new SysWeixin();
            weixin.MPAppId = WeixinApi.AppId;
            weixin.OpenId = openId;
            weixin.UserId = userId;

            using var http = new HttpClient();
            var user = await http.GetUserInfoAsync(openId);
            if (user != null)
            {
                weixin.UnionId = user.UnionId;
                weixin.Note = user.Note;
            }

            db.User = await UserRepository.GetUserInfoByIdAsync(db, userId);
            await db.SaveAsync(weixin);
            return db.User?.Name;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return string.Empty;
        }
    }

    public static async Task<string> UnsubscribeAsync(string openId, string userId)
    {
        try
        {
            var db = new Database();
            var weixin = await WeixinRepository.GetWeixinByOpenIdAsync(db, openId);
            if (weixin == null)
                return string.Empty;

            weixin.UserId = "";
            db.User = await UserRepository.GetUserInfoByIdAsync(db, userId);
            await db.SaveAsync(weixin);
            return db.User?.Name;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return string.Empty;
        }
    }

    public Task<Result> SendTemplateMessageAsync(TemplateInfo info)
    {
        var result = WeixinApi.SendTemplateMessage(info);
        return Task.FromResult(result);
    }
    #endregion
}