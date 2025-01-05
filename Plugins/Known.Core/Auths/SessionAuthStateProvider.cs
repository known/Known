namespace Known.Auths;

class SessionAuthStateProvider(JSService js) : AuthenticationStateProvider, IAuthStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var user = await js.GetUserInfoAsync();
        var info = Cache.GetUser(user?.UserName);
        if (info == null)
        {
            await SetCurrentUser(null);
            return null;
        }

        return user;
    }

    public Task SignInAsync(UserInfo user) => SetCurrentUser(user);
    public Task SignOutAsync() => SetCurrentUser(null);

    private async Task SetCurrentUser(UserInfo user)
    {
        await js.SetUserInfoAsync(user);
        var principal = GetPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal(Constant.KeyAuth);
    }
}