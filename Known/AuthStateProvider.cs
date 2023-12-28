using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Known;

public interface IAuthStateProvider
{
    Task<UserInfo> GetUserAsync();
    Task UpdateUserAsync(UserInfo user);
}

class WebAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private const string KeyUser = "Known_User";
    private readonly ProtectedSessionStorage sessionStorage;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public WebAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        this.sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = new AuthenticationState(anonymous);
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        var user = result.Success ? result.Value : null;
        if (user != null)
        {
            var principal = GetPrincipal(user);
            state = new AuthenticationState(principal);
        }
        return state;
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        return result.Value;
    }

    public async Task UpdateUserAsync(UserInfo user)
    {
        ClaimsPrincipal principal;

        if (user != null)
        {
            await sessionStorage.SetAsync(KeyUser, user);
            principal = GetPrincipal(user);
        }
        else
        {
            await sessionStorage.DeleteAsync(KeyUser);
            principal = anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        }, "Known_Auth"));
    }
}

class WinAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private static UserInfo current;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = new AuthenticationState(anonymous);
        if (current != null)
        {
            var principal = GetPrincipal(current);
            state = new AuthenticationState(principal);
        }
        return Task.FromResult(state);
    }

    public Task<UserInfo> GetUserAsync() => Task.FromResult(current);

    public Task UpdateUserAsync(UserInfo user)
    {
        var principal = anonymous;
        current = user;

        if (user != null)
            principal = GetPrincipal(user);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        }, "Known_Auth"));
    }
}