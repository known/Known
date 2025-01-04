namespace Known.Services;

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
    Task<SysWeixin> GetWeixinByUserIdAsync(string userId);

    /// <summary>
    /// 异步检查和关联系统微信用户。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> CheckWeixinAsync(UserInfo user);

    /// <summary>
    /// 异步保存微信用户信息。
    /// </summary>
    /// <param name="model">微信用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveWeixinAsync(WeixinInfo model);
}

[WebApi]
class WeixinService(Context context) : ServiceBase(context), IWeixinService
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