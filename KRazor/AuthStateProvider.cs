using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Known.Razor;

public class AuthStateProvider : AuthenticationStateProvider
{
    private const string KeyUser = "Known_User";
    private readonly ProtectedSessionStorage sessionStorage;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public AuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        this.sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
            var user = result.Success ? result.Value : null;
            if (user == null)
                return await Task.FromResult(new AuthenticationState(anonymous));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            }, "Known_Auth"));
            return await Task.FromResult(new AuthenticationState(principal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }
    }

    public void UpdateAuthenticationState(UserInfo user)
    {
        ClaimsPrincipal principal;

        if (user != null)
        {
            sessionStorage.SetAsync(KeyUser, user);
            principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            }));
        }
        else
        {
            sessionStorage.DeleteAsync(KeyUser);
            principal = anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
