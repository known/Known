/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using Known;

namespace KAdmin;

public class DataService : ServiceBase
{
    public Result Login(string userName, string password, string captcha)
    {
        return Platform.SignIn(userName, password, captcha);
    }

    public string GetCaptchaUrl()
    {
        var bytes = ImgUtils.CreateCaptcha(4, out string code);
        return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
    }
}