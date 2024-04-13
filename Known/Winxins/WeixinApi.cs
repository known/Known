namespace Known.Winxins;

class WeixinApi
{
    //获取ACCESS_TOKEN，7200秒过期，需要定时刷新才能调用接口
    //GET: https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
    //{"access_token":"ACCESS_TOKEN","expires_in":7200}

    #region 网页授权
    //1.用户同意授权，获取code
    //  用意返回redirect_uri/?code=CODE&state=STATE
    public static string GetAuthorizeUrl(string appId, string redirectUri, string state, string scope = "snsapi_userinfo")
    {
        return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri={redirectUri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
    }

    //2.通过code换取网页授权access_token
    /*  返回JSON
{
  "access_token":"ACCESS_TOKEN",
  "expires_in":7200,
  "refresh_token":"REFRESH_TOKEN",
  "openid":"OPENID",
  "scope":"SCOPE",
  "is_snapshotuser": 1,
  "unionid": "UNIONID"
}
     */
    public static string GetAuthorizeToken(string appId, string secret, string code)
    {
        return $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={secret}&code={code}&grant_type=authorization_code";
    }

    //3.刷新access_token（如果需要）
    /*  返回JSON
{ 
  "access_token":"ACCESS_TOKEN",
  "expires_in":7200,
  "refresh_token":"REFRESH_TOKEN",
  "openid":"OPENID",
  "scope":"SCOPE" 
}
     */
    public static string GetAuthorizeRefreshToken(string appId, string refreshToken)
    {
        return $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={appId}&grant_type=refresh_token&refresh_token={refreshToken}";
    }

    //4.拉取用户信息(需scope为 snsapi_userinfo)
    /*  返回JSON
{   
  "openid": "OPENID",
  "nickname": NICKNAME,
  "sex": 1,
  "province":"PROVINCE",
  "city":"CITY",
  "country":"COUNTRY",
  "headimgurl":"https://thirdwx.qlogo.cn/mmopen/g3MonUZtNHkdmzicIlibx6iaFqAc56vxLSUfpb6n5WKSYVY0ChQKkiaJSgQ1dZuTOgvLLrhJbERQQ4eMsv84eavHiaiceqxibJxCfHe/46",
  "privilege":[ "PRIVILEGE1" "PRIVILEGE2"     ],
  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
}
     */
    public static string GetUserInfo(string accessToken, string openId)
    {
        return $"https://api.weixin.qq.com/sns/userinfo?access_token={accessToken}&openid={openId}&lang=zh_CN";
    }
    #endregion
}