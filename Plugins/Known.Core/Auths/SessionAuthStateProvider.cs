namespace Known.Auths;

class SessionAuthStateProvider(JSService js, SessionManager session) : AuthenticationStateProvider, IAuthStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public Task<UserInfo> GetUserAsync() => js.GetUserInfoAsync();

    public async Task<string> SignInAsync(UserInfo user)
    {
        session.CreateSession(user);
        await SetCurrentUser(user);
        return user?.SessionId;
    }

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