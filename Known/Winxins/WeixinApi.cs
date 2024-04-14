using System.Net.Http.Json;
using Known.Extensions;

namespace Known.Winxins;

static class WeixinApi
{
    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //GET: https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}

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
        var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
        var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
        if (result == null)
        {
            Logger.Info("[WeixinApi] GetAuthorizeToken NULL.");
            return null;
        }

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

    //3.刷新access_token（如果需要）
    public static async Task<AuthorizeRefreshToken> GetAuthorizeRefreshTokenAsync(this HttpClient http, string appId, string refreshToken)
    {
        var url = $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={appId}&grant_type=refresh_token&refresh_token={refreshToken}";
        var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
        if (result == null)
        {
            Logger.Info("[WeixinApi] GetAuthorizeRefreshToken NULL.");
            return null;
        }

        return new AuthorizeRefreshToken
        {
            AccessToken = result.GetValue<string>("access_token"),
            ExpiresIn = result.GetValue<int>("expires_in"),
            RefreshToken = result.GetValue<string>("refresh_token"),
            OpenId = result.GetValue<string>("openid"),
            Scope = result.GetValue<string>("scope")
        };
    }

    //4.拉取用户信息(需scope为 snsapi_userinfo)
    public static async Task<SysWinxin> GetUserInfoAsync(this HttpClient http, string accessToken, string openId)
    {
        var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openId}&lang=zh_CN";
        var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
        if (result == null)
        {
            Logger.Info("[WeixinApi] GetUserInfo NULL.");
            return null;
        }

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

    //附：检验授权凭证（access_token）是否有效
    //正确返回：{ "errcode":0,"errmsg":"ok"}
    //错误返回：{ "errcode":40003,"errmsg":"invalid openid"}
    public static async Task<Result> CheckAccessTokenAsync(this HttpClient http, string accessToken, string openId)
    {
        var url = $"https://api.weixin.qq.com/sns/auth?access_token={accessToken}&openid={openId}";
        var result = await http.GetFromJsonAsync<Dictionary<string, object>>(url);
        if (result == null)
        {
            Logger.Info("[WeixinApi] CheckAccessToken NULL.");
            return null;
        }

        var errCode = result.GetValue<int>("errcode");
        var errMsg = result.GetValue<string>("errmsg");
        if (errCode != 0)
            return Result.Error(errMsg);

        return Result.Success(errMsg);
    }
    #endregion
}

public class AuthorizeToken
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public string OpenId { get; set; }
    public string Scope { get; set; }
    public int IsSnapshotUser { get; set; }
    public string UnionId { get; set; }
}

public class AuthorizeRefreshToken
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public string OpenId { get; set; }
    public string Scope { get; set; }
}