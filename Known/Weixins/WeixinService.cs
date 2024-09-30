namespace Known.Weixins;

/// <summary>
/// 微信服务接口。
/// </summary>
public interface IWeixinService : IService
{
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

class WeixinService(Context context) : ServiceBase(context), IWeixinService
{
    internal const string KeyWeixin = "WeixinInfo";

    #region Weixin
    public async Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        var database = Database;
        var info = await SystemService.GetConfigAsync<WeixinInfo>(database, KeyWeixin);
        if (info != null && !string.IsNullOrWhiteSpace(userId))
            info.User = await GetWeixinByUserIdAsync(database, userId);
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

    internal static async Task<Result> SendTemplateMessageAsync(Database db, WeixinTemplateInfo info)
    {
        var task = WeixinHelper.CreateTask(info);
        await db.SaveAsync(task);
        return Result.Success("Task saved！");
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
        var weixin = await GetWeixinByUserIdAsync(database, user.Token);
        if (weixin == null)
            return null;

        weixin.UserId = user.Id;
        await database.SaveAsync(weixin);
        user.OpenId = weixin.OpenId;
        return user;
    }
    #endregion
}