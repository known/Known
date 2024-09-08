namespace Known.Weixins;

/// <summary>
/// 微信帮助者类。
/// </summary>
public sealed class WeixinHelper
{
    private WeixinHelper() { }

    /// <summary>
    /// 异步网页授权操作。
    /// </summary>
    /// <param name="token">系统用户token。</param>
    /// <param name="code">网页授权码。</param>
    /// <returns>授权结果。</returns>
    public static async Task<Result> AuthorizeAsync(string token, string code)
    {
        using var http = new HttpClient();
        var authToken = await http.GetAuthorizeTokenAsync(code);
        if (authToken == null || string.IsNullOrWhiteSpace(authToken.AccessToken))
            return Result.Error("AccessToken is null.");

        var user = await http.GetUserInfoAsync(authToken.AccessToken, authToken.OpenId);
        if (user == null)
            return Result.Error("WeixinUser is null.");

        var db = Database.Create();
        db.User = AuthService.GetUserByToken(token);
        var weixin = await GetWeixinByOpenIdAsync(db, user.OpenId);
        if (weixin == null)
        {
            user.MPAppId = WeixinApi.AppId;
            user.UserId = token;
            await db.InsertAsync(user);
        }
        return Result.Success("Authorize OK!");
    }

    /// <summary>
    /// 异步关注公众号。
    /// </summary>
    /// <param name="openId">用户OpenId。</param>
    /// <param name="userId">系统用户ID。</param>
    /// <returns>系统用户姓名。</returns>
    public static async Task<string> SubscribeAsync(string openId, string userId)
    {
        try
        {
            var db = Database.Create();
            var weixin = await GetWeixinByOpenIdAsync(db, openId);
            weixin ??= new SysWeixin();
            weixin.MPAppId = WeixinApi.AppId;
            weixin.OpenId = openId;
            weixin.UserId = userId;

            using var http = new HttpClient();
            var user = await http.GetUserInfoAsync(openId);
            if (user != null)
            {
                weixin.UnionId = user.UnionId;
                weixin.Note = user.Note;
            }

            db.User = await UserHelper.GetUserByIdAsync(db, userId);
            await db.SaveAsync(weixin);
            return db.User?.Name;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return string.Empty;
        }
    }

    /// <summary>
    /// 异步取消关注公众号。
    /// </summary>
    /// <param name="openId">用户OpenId。</param>
    /// <param name="userId">系统用户ID。</param>
    /// <returns>系统用户姓名。</returns>
    public static async Task<string> UnsubscribeAsync(string openId, string userId)
    {
        try
        {
            var db = Database.Create();
            var weixin = await GetWeixinByOpenIdAsync(db, openId);
            if (weixin == null)
                return string.Empty;

            weixin.UserId = "";
            db.User = await UserHelper.GetUserByIdAsync(db, userId);
            await db.SaveAsync(weixin);
            return db.User?.Name;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return string.Empty;
        }
    }

    private static Task<SysWeixin> GetWeixinByOpenIdAsync(Database db, string openId)
    {
        return db.QueryAsync<SysWeixin>(d => d.OpenId == openId);
    }
}