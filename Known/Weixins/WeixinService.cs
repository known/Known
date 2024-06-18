namespace Known.Weixins;

public interface IWeixinService : IService
{
    Task<WeixinInfo> GetWeixinAsync();
    Task<SysWeixin> GetWeixinByUserIdAsync(string userId);
    Task<string> GetQRCodeUrlAsync(string sceneId);
    Task<UserInfo> CheckWeixinAsync(UserInfo user);
    Task<Result> SaveWeixinAsync(WeixinInfo model);
}

class WeixinClient(HttpClient http) : ClientBase(http), IWeixinService
{
    public Task<WeixinInfo> GetWeixinAsync() => GetAsync<WeixinInfo>("Weixin/GetWeixin");
    public Task<SysWeixin> GetWeixinByUserIdAsync(string userId) => GetAsync<SysWeixin>($"Weixin/GetWeixinByUserId?userId={userId}");
    public Task<string> GetQRCodeUrlAsync(string sceneId) => GetAsync<string>($"Weixin/GetQRCodeUrl?sceneId={sceneId}");
    public Task<UserInfo> CheckWeixinAsync(UserInfo user) => PostAsync<UserInfo, UserInfo>("Weixin/CheckWeixin", user);
    public Task<Result> SaveWeixinAsync(WeixinInfo model) => PostAsync("Weixin/SaveWeixin", model);
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

    //public Task<SysWeixin> GetWeixinByOpenIdAsync(string openId)
    //{
    //    return WeixinRepository.GetWeixinByOpenIdAsync(Database, openId);
    //}

    public Task<SysWeixin> GetWeixinByUserIdAsync(string userId)
    {
        return WeixinRepository.GetWeixinByUserIdAsync(Database, userId);
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

    //public async Task<string> GetAuthorizeUrlAsync(string state)
    //{
    //    var info = await SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
    //    if (info == null || !info.IsWeixinAuth)
    //        return string.Empty;

    //    return WeixinApi.GetAuthorizeUrl(state);
    //}

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
    #endregion
}