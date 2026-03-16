namespace Known.Auths;

class SessionAuthStateProvider(JSService js, SessionManager session) : AuthenticationStateProvider, IAuthStateProvider
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
        if (user == null)
            return null;

        CoreConfig.Systems.TryGetValue(user.CompNo, out SystemInfo sys);
        if (sys?.IsLoginOne == true && !session.ValidateSession(user.UserName, user.SessionId))
        {
            await SetCurrentUser(null);
            return null;
        }

        return user;
    }

    public async Task<string> SignInAsync(UserInfo user)
    {
        await session.CreateSessionAsync(user);
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