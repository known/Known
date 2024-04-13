namespace Known.Winxins;

class WeixinApi
{
    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //GET: https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}

    //网页登录
    public static string GetAuthorizeUrl(string appId, string redirectUri, string state, string scope = "snsapi_userinfo")
    {
        return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
    }
}