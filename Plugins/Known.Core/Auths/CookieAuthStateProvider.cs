namespace Known.Auths;

class CookieAuthStateProvider(IHttpContextAccessor context, IAdminService platform, SessionManager session) : AuthenticationStateProvider, IAuthStateProvider
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

    public async Task<string> SignInAsync(UserInfo user)
    {
        var principal = GetPrincipal(user);
        if (user != null)
        {
            session.CreateSession(user);
            await context.HttpContext.SignInAsync(
                Constant.KeyAuth, principal,
                new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(1) }
            );
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return user?.SessionId;
    }

    public async Task SignOutAsync()
    {
        var principal = GetPrincipal(null);
        await context.HttpContext.SignOutAsync(Constant.KeyAuth);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal(Constant.KeyAuth);
    }
}