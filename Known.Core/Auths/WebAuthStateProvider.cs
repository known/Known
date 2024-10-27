namespace Known.Core.Auths;

class WebAuthStateProvider(IHttpContextAccessor context) : AuthenticationStateProvider, IAuthStateProvider
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

        var db = Database.Create();
        var userName = context.HttpContext.User.Identity.Name;
        return AuthService.GetUserAsync(db, userName);
    }

    public async Task SetUserAsync(UserInfo user)
    {
        var principal = GetPrincipal(user);
        if (user != null)
            await context.HttpContext.SignInAsync(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal();
    }
}