namespace Known.Services;

partial class AdminService
{
    internal const string KeyWeixin = "WeixinInfo";

    public Task<string> GetQRCodeUrlAsync(string sceneId) => WeixinApi.GetQRCodeUrlAsync(sceneId);

    #region Weixin
    public async Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        var database = Database;
        var info = await database.GetConfigAsync<WeixinInfo>(KeyWeixin);
        if (info != null && !string.IsNullOrWhiteSpace(userId))
            info.User = await database.GetWeixinAsync(userId);
        return info;
    }

    public async Task<Result> SaveWeixinAsync(WeixinInfo model)
    {
        await Database.SaveConfigAsync(KeyWeixin, model);
        return Result.Success(Language.Success(Language.Save));
    }

    //public Task<SysWeixin> GetWeixinByOpenIdAsync(string openId)
    //{
    //    return WeixinHelper.GetWeixinByOpenIdAsync(Database, openId);
    //}

    public Task<SysWeixin> GetWeixinByUserIdAsync(string userId)
    {
        return Database.GetWeixinAsync(userId);
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
        var database = Database;
        var weixin = await database.GetWeixinAsync(user.Token);
        if (weixin == null)
            return null;

        weixin.UserId = user.Id;
        await database.SaveAsync(weixin);
        user.OpenId = weixin.OpenId;
        return user;
    }
    #endregion
}