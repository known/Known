namespace Known.Weixins;

/// <summary>
/// 微信服务接口。
/// </summary>
public interface IWeixinService : IService
{
    /// <summary>
    /// 异步获取关注公众号，绑定用户的二维码。
    /// </summary>
    /// <param name="sceneId">场景ID。</param>
    /// <returns>绑定二维码URL。</returns>
    Task<string> GetQRCodeUrlAsync(string sceneId);

    /// <summary>
    /// 异步获取微信配置信息。
    /// </summary>
    /// <param name="userId">系统用户ID。</param>
    /// <returns>微信配置信息。</returns>
    Task<WeixinInfo> GetWeixinAsync(string userId);

    /// <summary>
    /// 异步获取微信用户信息。
    /// </summary>
    /// <param name="userId">系统用户ID。</param>
    /// <returns>微信用户信息。</returns>
    Task<WeixinUserInfo> GetWeixinByUserIdAsync(string userId);

    /// <summary>
    /// 异步检查和关联系统微信用户。
    /// </summary>
    /// <param name="info">用户信息。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> CheckWeixinAsync(UserInfo info);

    /// <summary>
    /// 异步保存微信用户信息。
    /// </summary>
    /// <param name="info">微信用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveWeixinAsync(WeixinInfo info);
}

[Client]
class WeixinClient(HttpClient http) : ClientBase(http), IWeixinService
{
    public Task<string> GetQRCodeUrlAsync(string sceneId) => Http.GetTextAsync($"/Weixin/GetQRCodeUrl?sceneId={sceneId}");
    public Task<WeixinInfo> GetWeixinAsync(string userId) => Http.GetAsync<WeixinInfo>($"/Weixin/GetWeixin?userId={userId}");
    public Task<WeixinUserInfo> GetWeixinByUserIdAsync(string userId) => Http.GetAsync<WeixinUserInfo>($"/Weixin/GetWeixinByUserId?userId={userId}");
    public Task<UserInfo> CheckWeixinAsync(UserInfo info) => Http.PostAsync<UserInfo, UserInfo>("/Weixin/CheckWeixin", info);
    public Task<Result> SaveWeixinAsync(WeixinInfo info) => Http.PostAsync("/Weixin/SaveWeixin", info);
}

[WebApi, Service]
class WeixinService(Context context) : ServiceBase(context), IWeixinService
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