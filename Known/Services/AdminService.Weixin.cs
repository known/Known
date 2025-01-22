namespace Known.Services;

public partial interface IAdminService
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

partial class AdminService
{
    public Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        return Task.FromResult("");
    }

    public Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        return Task.FromResult(new WeixinInfo());
    }

    public Task<WeixinUserInfo> GetWeixinByUserIdAsync(string userId)
    {
        return Task.FromResult(new WeixinUserInfo());
    }

    public Task<UserInfo> CheckWeixinAsync(UserInfo info)
    {
        return Task.FromResult(new UserInfo());
    }

    public Task<Result> SaveWeixinAsync(WeixinInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class AdminClient
{
    public Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        return Http.GetStringAsync($"/Admin/GetQRCodeUrl?sceneId={sceneId}");
    }

    public Task<WeixinInfo> GetWeixinAsync(string userId)
    {
        return Http.GetAsync<WeixinInfo>($"/Admin/GetWeixin?userId={userId}");
    }

    public Task<WeixinUserInfo> GetWeixinByUserIdAsync(string userId)
    {
        return Http.GetAsync<WeixinUserInfo>($"/Admin/GetWeixinByUserId?userId={userId}");
    }

    public Task<UserInfo> CheckWeixinAsync(UserInfo info)
    {
        return Http.PostAsync<UserInfo, UserInfo>("/Admin/CheckWeixin", info);
    }

    public Task<Result> SaveWeixinAsync(WeixinInfo info)
    {
        return Http.PostAsync("/Admin/SaveWeixin", info);
    }
}