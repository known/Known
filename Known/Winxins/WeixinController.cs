using Microsoft.AspNetCore.Mvc;

namespace Known.Winxins;

[Route("[controller]")]
public class WeixinController : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<string> Index([FromQuery] string token, [FromQuery] string code)
    {
        Logger.Info($"token={token}");
        Logger.Info($"code={code}");

        var http = new HttpClient();
        var authToken = await http.GetAuthorizeTokenAsync(WeixinHelper.AppId, WeixinHelper.AppSecret, code);
        if (authToken == null)
            return "AuthToken is null.";

        var user = await http.GetUserInfoAsync(authToken.AccessToken, authToken.OpenId);
        if (user == null)
            return "WeixinUser is null.";

        var db = new Database();
        var weixin = await WeixinRepository.GetWinxinByOpenIdAsync(db, user.OpenId);
        if (weixin == null)
        {
            user.MPAppId = WeixinHelper.AppId;
            user.UserId = token;
            await db.InsertAsync(user);
        }
        return "OK";
    }
}