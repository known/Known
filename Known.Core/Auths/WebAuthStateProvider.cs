namespace Known.Core.Auths;

class WebAuthStateProvider(JSService js) : AuthenticationStateProvider, IAuthStateProvider
{
    private readonly JSService js = js;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await js.GetUserInfoAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public Task<UserInfo> GetUserAsync() => js.GetUserInfoAsync();

    public async Task SetUserAsync(UserInfo user)
    {
        await js.SetUserInfoAsync(user);
        var principal = GetPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal();
    }
}