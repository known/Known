namespace Known.Core.Auths;

class CookieAuthStateProvider(IHttpContextAccessor context, IPlatformService platform) : AuthenticationStateProvider, IAuthStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public Task<UserInfo> GetUserAsync()
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
            return Task.FromResult(default(UserInfo));

        var userName = context.HttpContext.User.Identity.Name;
        return AuthHelper.GetUserAsync(platform, userName);
    }

    public async Task SignInAsync(UserInfo user)
    {
        var principal = GetPrincipal(user);
        if (user != null)
        {
            await context.HttpContext.SignInAsync(
                Constants.KeyAuth, principal,
                new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(1) }
            );
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task SignOutAsync()
    {
        var principal = GetPrincipal(null);
        await context.HttpContext.SignOutAsync(Constants.KeyAuth);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal(Constants.KeyAuth);
    }
}