using System.Net.Http.Json;
using Known.Extensions;

namespace Known.Winxins;

static class WeixinApi
{
    private static string AccessToken = "";

    #region 初始化接口
    internal static void Initialize()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var appId = WeixinHelper.AppId;
                var appSecret = WeixinHelper.AppSecret;
                if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(appSecret))
                    AccessToken = await GetAccessTokenAsync(appId, appSecret);
                Thread.Sleep(7000 * 1000);
            }
        });
    }

    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}
    private static async Task<string> GetAccessTokenAsync(string appId, string appSecret)
    {
        try
        {
            var http = new HttpClient();
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            return result.GetValue<string>("access_token");
        }
        catch(Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }
    #endregion

    #region 网页授权
    //1.用户同意授权，获取code
    //  同意返回redirect_uri/?code=CODE&state=STATE
    public static string GetAuthorizeUrl(string appId, string redirectUri, string state, string scope = "snsapi_userinfo")
    {
        return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
    }

    //2.通过code换取网页授权access_token
    public static async Task<AuthorizeToken> GetAuthorizeTokenAsync(this HttpClient http, string appId, string secret, string code)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
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
            Logger.Exception(ex);
            return null;
        }
    }

    //3.刷新access_token（如果需要）
    public static async Task<AuthorizeRefreshToken> GetAuthorizeRefreshTokenAsync(this HttpClient http, string appId, string refreshToken)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={appId}&grant_type=refresh_token&refresh_token={refreshToken}";
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
            Logger.Exception(ex);
            return null;
        }
    }

    //4.拉取用户信息(需scope为 snsapi_userinfo)
    public static async Task<SysWinxin> GetUserInfoAsync(this HttpClient http, string accessToken, string openId)
    {
        try
        {
            var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openId}&lang=zh_CN";
            var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
            var privileges = result.GetValue<List<string>>("privilege");
            var info = new SysWinxin
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
            Logger.Exception(ex);
            return null;
        }
    }

    //附：检验授权凭证（access_token）是否有效
    //正确返回：{ "errcode":0,"errmsg":"ok"}
    //错误返回：{ "errcode":40003,"errmsg":"invalid openid"}
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
            Logger.Exception(ex);
            return null;
        }
    }
    #endregion

    #region 模板消息
    //发送模板消息
    public static async Task<Result> SendTemplateMessageAsync(this HttpClient http, TemplateInfo info)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
            return null;

        try
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={AccessToken}";
            var response = await http.PostAsJsonAsync(url, info);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            var errCode = result.GetValue<int>("errcode");
            var errMsg = result.GetValue<string>("errmsg");
            if (errCode != 0)
                return Result.Error(errMsg);

            return Result.Success(errMsg);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }
    #endregion
}