namespace Known.Services;

partial class AdminService
{
    internal const string KeyWeixin = "WeixinInfo";

    public async Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        var user = CurrentUser;
        var weixin = await GetWeixinAsync(user.Id);
        if (weixin == null || !weixin.IsWeixinAuth)
            return string.Empty;

        if (weixin.User != null)
            return string.Empty;

        //扫码场景ID：{场景ID}_{用户ID}
        return await WeixinApi.GetQRCodeUrlAsync($"{sceneId}_{user.Id}");
    }

    #region Weixin
    public async Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        var database = Database;
        var info = await database.GetConfigAsync<WeixinInfo>(KeyWeixin);
        if (info != null && !string.IsNullOrWhiteSpace(userId))
            info.User = await database.GetWeixinUserAsync(userId);
        return info;
    }

    public async Task<Result> SaveWeixinAsync(WeixinInfo info)
    {
        await Database.SaveConfigAsync(KeyWeixin, info);
        return Result.Success(Language.SaveSuccess);
    }

    //public Task<SysWeixin> GetWeixinByOpenIdAsync(string openId)
    //{
    //    return WeixinHelper.GetWeixinByOpenIdAsync(Database, openId);
    //}

    public Task<WeixinUserInfo> GetWeixinByUserIdAsync(string userId)
    {
        return Database.GetWeixinUserAsync(userId);
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

    public async Task<UserInfo> CheckWeixinAsync(UserInfo info)
    {
        // 检查用户是否扫码关注成功
        var database = Database;
        var weixin = await database.QueryAsync<SysWeixin>(d => d.UserId == info.Token);
        if (weixin == null)
            return null;

        weixin.UserId = info.Id;
        await database.SaveAsync(weixin);
        info.OpenId = weixin.OpenId;
        return info;
    }
    #endregion
}