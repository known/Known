using Microsoft.AspNetCore.Authentication;

namespace Known.Core.Auths;

class WebApiAuthStateProvider(HttpContext context) : AuthenticationStateProvider, IAuthStateProvider
{
    private const string KeyUser = "Known_User";
    private readonly HttpContext context = context;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public Task<UserInfo> GetUserAsync()
    {
        //var userName = context.User.Identity.Name;
        return Task.FromResult(new UserInfo());
    }

    public async Task SetUserAsync(UserInfo user)
    {
        var principal = GetPrincipal(user);
        await context.SignInAsync(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal();
    }
}