namespace Known.Weixins;

public interface IWeixinService : IService
{
    Task<WeixinInfo> GetWeixinAsync(string userId);
    Task<SysWeixin> GetWeixinByUserIdAsync(string userId);
    Task<UserInfo> CheckWeixinAsync(UserInfo user);
    Task<Result> SaveWeixinAsync(WeixinInfo model);
}

class WeixinService(Context context) : ServiceBase(context), IWeixinService
{
    internal const string KeyWeixin = "WeixinInfo";

    #region Weixin
    public async Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        var info = await SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
        if (info != null && !string.IsNullOrWhiteSpace(userId))
            info.User = await GetWeixinByUserIdAsync(Database, userId);
        return info;
    }

    public async Task<Result> SaveWeixinAsync(WeixinInfo model)
    {
        await SystemService.SaveConfigAsync(Database, KeyWeixin, model);
        return Result.Success(Language.Success(Language.Save));
    }

    //public Task<SysWeixin> GetWeixinByOpenIdAsync(string openId)
    //{
    //    return WeixinHelper.GetWeixinByOpenIdAsync(Database, openId);
    //}

    public Task<SysWeixin> GetWeixinByUserIdAsync(string userId)
    {
        return GetWeixinByUserIdAsync(Database, userId);
    }

    internal static Task<SysWeixin> GetWeixinByUserIdAsync(Database db, string userId)
    {
        return db.QueryAsync<SysWeixin>(d => d.UserId == userId);
    }
    #endregion

    #region MP
    //public async Task<string> GetAuthorizeUrlAsync(string state)
    //{
    //    var info = await SystemService.GetConfigAsync<WeixinInfo>(Database, KeyWeixin);
    //    if (info == null || !info.IsWeixinAuth)
    //        return string.Empty;

    //    return WeixinApi.GetAuthorizeUrl(state);
    //}

    public async Task<UserInfo> CheckWeixinAsync(UserInfo user)
    {
        var weixin = await GetWeixinByUserIdAsync(Database, user.Token);
        if (weixin == null)
            return null;

        weixin.UserId = user.Id;
        await Database.SaveAsync(weixin);
        user.OpenId = weixin.OpenId;
        return user;
    }
    #endregion
}