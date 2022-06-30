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

using System.Security.Claims;
using Known.Razor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KAdmin;

public class BasePage : PageComponent
{
    [Inject] protected DataService Service { get; set; }

    protected bool IsAdmin
    {
        get { return Client.CheckIsAdmin(CurrentUser); }
    }
}

public class LoginModel : PageModel
{
    public async Task<IActionResult> OnGetAsync(string userName, string password)
    {
        string returnUrl = Url.Content("~/");
        try
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        catch
        {
        }

        var claims = new List<Claim>
        {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "Administrator"),
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = Request.Host.Value
        };
        try
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
        catch (Exception)
        {
        }
        return LocalRedirect(returnUrl);
    }
}
