namespace Known.Auths;

class CookieAuthStateProvider(IHttpContextAccessor context, IAdminService platform, SessionManager session) : AuthenticationStateProvider, IAuthStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public async Task<UserInfo> GetUserAsync()
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
            return default;

        var userName = context.HttpContext.User.Identity.Name;
        var user = await AuthHelper.GetUserAsync(platform, userName);
        if (user == null)
            return null;

        CoreConfig.Systems.TryGetValue(user.CompNo, out SystemInfo sys);
        if (sys?.IsLoginOne == true)
        {
            var sessionId = context.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            if (string.IsNullOrWhiteSpace(sessionId) || !session.ValidateSession(userName, sessionId))
            {
                await SignOutAsync();
                return null;
            }
        }

        return user;
    }

    public async Task<string> SignInAsync(UserInfo user)
    {
        if (user != null)
        {
            await session.CreateSessionAsync(user);
            var principal1 = GetPrincipal(user);
            await context.HttpContext.SignInAsync(
                Constant.KeyAuth, principal1,
                new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(1) }
            );
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal1)));
            return user.SessionId;
        }

        var principal = GetPrincipal(null);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return null;
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