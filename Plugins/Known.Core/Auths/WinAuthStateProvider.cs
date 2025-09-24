namespace Known.Auths;

class WinAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private static UserInfo current;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync();
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public Task<UserInfo> GetUserAsync() => Task.FromResult(current);

    public async Task<string> SignInAsync(UserInfo user)
    {
        if (user == null)
            return string.Empty;

        user.SessionId = Utils.GetGuid();
        await SetCurrentUser(user);
        return user.SessionId;
    }

    public async Task SignOutAsync()
    {
        await SetCurrentUser(null);
    }

    private Task SetCurrentUser(UserInfo user)
    {
        current = user;
        var principal = GetPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal(Constant.KeyAuth);
    }
}