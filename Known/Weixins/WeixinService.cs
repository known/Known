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
    public Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        throw new NotImplementedException();
    }

    public Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<SysWeixin> GetWeixinByUserIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserInfo> CheckWeixinAsync(UserInfo user)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveWeixinAsync(WeixinInfo model)
    {
        throw new NotImplementedException();
    }
}