using System.Net.Http.Json;
using System.Web;

namespace Known.Weixins;

/// <summary>
/// 微信Api操作类。
/// </summary>
public static class WeixinApi
{
    private static string AccessToken = "";
    private static DateTime RefreshTime = DateTime.MinValue;

    /// <summary>
    /// 取得公众号ID。
    /// </summary>
    public static string GZHId { get; private set; }

    /// <summary>
    /// 取得微信公众号AppId。
    /// </summary>
    public static string AppId { get; private set; }

    /// <summary>
    /// 取得微信公众号安全密钥。
    /// </summary>
    public static string AppSecret { get; private set; }

    /// <summary>
    /// 取得微信公众绑定的服务器URL。
    /// </summary>
    public static string RedirectUri { get; private set; }

    #region 初始化接口
    /// <summary>
    /// 刷新微信访问Token。
    /// </summary>
    /// <returns></returns>
    public static async Task RefreshTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(AppSecret))
            return;

        AccessToken = await GetAccessTokenAsync(AppId, AppSecret);
        if (!string.IsNullOrWhiteSpace(AccessToken))
            RefreshTime = DateTime.Now;
    }

    /// <summary>
    /// 初始化微信接口，定时刷新访问Token。
    /// </summary>
    public static void Initialize(WeixinConfigInfo info)
    {
        if (info == null)
            return;

        GZHId = info.GZHId;
        AppId = info.AppId;
        AppSecret = info.AppSecret;
        RedirectUri = info.RedirectUri;

        //ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        //Task.Run(async () =>
        //{
        //    while (true)
        //    {
        //        await RefreshTokenAsync();
        //        Thread.Sleep(7000 * 1000);
        //    }
        //});
    }

    //获取稳定版接口调用凭据
    //https://mmbizurl.cn/s/JtxxFh33r 
    //private static async Task<string> GetStableAccessTokenAsync(string appId, string appSecret)
    //{
    //    try
    //    {
    //        using var http = new HttpClient();
    //        var url = "https://api.weixin.qq.com/cgi-bin/stable_token";
    //        var data = new
    //        {
    //            grant_type = "client_credential",
    //            appid = appId,
    //            secret = appSecret,
    //            force_refresh = true
    //        };
    //        var result = await http.PostDataAsync(url, data);
    //        WriteInfo("AT=" + Utils.ToJson(result));
    //        return result.GetValue<string>("access_token");
    //    }
    //    catch (Exception ex)
    //    {
    //        WriteException(ex);
    //        return null;
    //    }
    //}

    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}
    private static async Task<string> GetAccessTokenAsync(string appId, string appSecret)
    {
        try
        {
            using var http = new HttpClient();
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            WriteInfo("AT=" + Utils.ToJson(result));
            return result.GetValue<string>("access_token");
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }

    //private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
    //{
    //    //为了通过证书验证，总是返回true
    //    return true;
    //}
    #endregion

    #region 二维码
    /// <summary>
    /// 异步获取关注公众号，绑定用户的二维码。
    /// </summary>
    /// <param name="sceneId">场景ID。</param>
    /// <returns>绑定二维码URL。</returns>
    public static async Task<string> GetQRCodeUrlAsync(string sceneId)
    {
        using var http = new HttpClient();
        var ticket = await http.CreateTicketAsync(sceneId);
        if (ticket == null)
            return string.Empty;

        return GetQRCodeUrl(ticket.Ticket);
    }
    #endregion

    #region 账号管理
    /// <summary>
    /// 1.异步创建二维码ticket。
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="sceneId">场景ID。</param>
    /// <returns>二维码ticket。</returns>
    public static async Task<TicketInfo> CreateTicketAsync(this HttpClient http, string sceneId)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            WriteInfo("AccessToken is null.");
            return null;
        }

        try
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={AccessToken}";
            var data = new
            {
                action_name = "QR_LIMIT_STR_SCENE",
                action_info = new { scene = new { scene_str = sceneId } }
            };
            var result = await http.PostDataAsync(url, data);
            if (result == null)
                return null;

            return new TicketInfo
            {
                Ticket = result.GetValue<string>("ticket"),
                ExpireSeconds = result.GetValue<int>("expire_seconds"),
                Url = result.GetValue<string>("url")
            };
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }

    /// <summary>
    /// 2.通过ticket换取二维码URL。
    /// </summary>
    /// <param name="ticket">二维码ticket。</param>
    /// <returns>二维码URL。</returns>
    private static string GetQRCodeUrl(string ticket)
    {
        ticket = HttpUtility.UrlEncode(ticket);
        return $"https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={ticket}";
    }

    /// <summary>
    /// 异步获取微信用户信息。
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="openId">用户OpenId。</param>
    /// <returns>微信用户信息。</returns>
    public static async Task<WeixinUserInfo> GetUserInfoAsync(this HttpClient http, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={AccessToken}&openid={openId}&lang=zh_CN";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var privileges = result.GetValue<List<string>>("privilege");
            var info = new WeixinUserInfo
            {
                OpenId = result.GetValue<string>("openid"),
                NickName = result.GetValue<string>("nickname"),
                Sex = result.GetValue<string>("sex"),
                Province = result.GetValue<string>("province"),
                City = result.GetValue<string>("city"),
                Country = result.GetValue<string>("country"),
                HeadImgUrl = result.GetValue<string>("headimgurl"),
                UnionId = result.GetValue<string>("unionid"),
                Note = result.GetValue<string>("remark")
            };
            if (privileges != null)
                info.Privilege = string.Join(",", privileges);
            return info;
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }
    #endregion

    #region 网页授权
    /// <summary>
    /// 1.用户同意授权，获取code。
    /// 同意返回redirect_uri/?code=CODE&amp;state=STATE
    /// </summary>
    /// <param name="state">自定义状态字符串。</param>
    /// <param name="scope">范围，默认：snsapi_userinfo。</param>
    /// <returns>跳转到业务系统的URL。</returns>
    public static string GetAuthorizeUrl(string state, string scope = "snsapi_userinfo")
    {
        var redirectUri = HttpUtility.UrlEncode(RedirectUri);
        state = HttpUtility.UrlEncode(state);
        //网页登录二维码
        return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={AppId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
    }

    /// <summary>
    /// 2.通过code换取网页授权access_token。
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="code">网页授权code。</param>
    /// <returns>微信认证Token。</returns>
    public static async Task<AuthorizeToken> GetAuthorizeTokenAsync(this HttpClient http, string code)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={AppId}&secret={AppSecret}&code={code}&grant_type=authorization_code";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return new AuthorizeToken
            {
                AccessToken = result.GetValue<string>("access_token"),
                ExpiresIn = result.GetValue<int>("expires_in"),
                RefreshToken = result.GetValue<string>("refresh_token"),
                OpenId = result.GetValue<string>("openid"),
                Scope = result.GetValue<string>("scope"),
                IsSnapshotUser = result.GetValue<int>("is_snapshotuser"),
                UnionId = result.GetValue<string>("unionid")
            };
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }

    /// <summary>
    /// 3.刷新access_token（如果需要）
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="refreshToken">刷新Token。</param>
    /// <returns>刷新Token。</returns>
    public static async Task<AuthorizeRefreshToken> GetAuthorizeRefreshTokenAsync(this HttpClient http, string refreshToken)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={AppId}&grant_type=refresh_token&refresh_token={refreshToken}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return new AuthorizeRefreshToken
            {
                AccessToken = result.GetValue<string>("access_token"),
                ExpiresIn = result.GetValue<int>("expires_in"),
                RefreshToken = result.GetValue<string>("refresh_token"),
                OpenId = result.GetValue<string>("openid"),
                Scope = result.GetValue<string>("scope")
            };
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }

    /// <summary>
    /// 4.拉取用户信息(需scope为 snsapi_userinfo)
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="accessToken">访问Token。</param>
    /// <param name="openId">用户OpenId。</param>
    /// <returns>微信用户信息。</returns>
    public static async Task<WeixinUserInfo> GetUserInfoAsync(this HttpClient http, string accessToken, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openId}&lang=zh_CN";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var privileges = result.GetValue<List<string>>("privilege");
            var info = new WeixinUserInfo
            {
                OpenId = result.GetValue<string>("openid"),
                NickName = result.GetValue<string>("nickname"),
                Sex = result.GetValue<string>("sex"),
                Province = result.GetValue<string>("province"),
                City = result.GetValue<string>("city"),
                Country = result.GetValue<string>("country"),
                HeadImgUrl = result.GetValue<string>("headimgurl"),
                UnionId = result.GetValue<string>("unionid")
            };
            if (privileges != null)
                info.Privilege = string.Join(",", privileges);
            return info;
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }

    /// <summary>
    /// 附：检验授权凭证（access_token）是否有效
    /// 正确返回：{ "errcode":0,"errmsg":"ok"}
    /// 错误返回：{ "errcode":40003,"errmsg":"invalid openid"}
    /// </summary>
    /// <param name="http">http客户端。</param>
    /// <param name="accessToken">访问Token。</param>
    /// <param name="openId">用户OpenId。</param>
    /// <returns>检验结果。</returns>
    public static async Task<Result> CheckAccessTokenAsync(this HttpClient http, string accessToken, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/auth?access_token={accessToken}&openid={openId}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var errCode = result.GetValue<int>("errcode");
            var errMsg = result.GetValue<string>("errmsg");
            if (errCode != 0)
                return Result.Error(errMsg);

            return Result.Success(errMsg);
        }
        catch (Exception ex)
        {
            WriteException(ex);
            return null;
        }
    }
    #endregion

    #region 模板消息
    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="info">模板消息对象。</param>
    /// <returns>发送结果。</returns>
    public static async Task<Result> SendTemplateMessageAsync(TemplateInfo info)
    {
        if (string.IsNullOrWhiteSpace(AccessToken) || RefreshTime.AddSeconds(3600) < DateTime.Now)
            await RefreshTokenAsync();

        if (string.IsNullOrWhiteSpace(AccessToken))
            return Result.Error("AccessToken is null.");

        try
        {
            using var http = new HttpClient();
            var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={AccessToken}";
            var result = await http.PostDataAsync(url, info, "application/x-www-form-urlencoded");
            var errCode = result?.GetValue<int>("errcode");
            var errMsg = result?.GetValue<string>("errmsg");
            if (errCode != 0)
                return Result.Error(errMsg);

            return Result.Success(errMsg);
        }
        catch (Exception ex)
        {
            WriteException(ex);
            WriteInfo(Utils.ToJson(info));
            return Result.Error(ex.Message);
        }
    }
    #endregion

    private static async Task<Dictionary<string, object>> PostDataAsync(this HttpClient http, string url, object data, string mediaType = "application/json")
    {
        var json = Utils.ToJson(data);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(json, Encoding.UTF8, mediaType);
        var response = await http.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = Utils.FromJson<Dictionary<string, object>>(content);
        if (result == null)
            WriteInfo($"PostData={content}");
        return result;
    }

    private static void WriteInfo(string message)
    {
        Logger.Information(LogTarget.BackEnd, new UserInfo { Name = "WeixinApi" }, message);
    }

    private static void WriteException(Exception ex)
    {
        Logger.Exception(LogTarget.BackEnd, new UserInfo { Name = "WeixinApi" }, ex);
    }
}