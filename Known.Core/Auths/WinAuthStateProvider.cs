namespace Known.Core.Auths;

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

    public Task SetUserAsync(UserInfo user)
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

        return user.ToPrincipal();
    }
}