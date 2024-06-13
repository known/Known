namespace Known.Weixins;

public interface IWeixinService : IService
{
    Task<WeixinInfo> GetWeixinAsync();
    Task<UserInfo> CheckWeixinAsync(UserInfo user);
    Task<Result> SaveWeixinAsync(WeixinInfo model);
}

class WeixinService(Context context) : ServiceBase(context), IWeixinService
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

    internal Task<Result> SendTemplateMessageAsync(TemplateInfo info)
    {
        var result = WeixinApi.SendTemplateMessage(info);
        return Task.FromResult(result);
    }
    #endregion
}