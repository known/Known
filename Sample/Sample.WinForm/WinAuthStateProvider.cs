using System.Security.Claims;

namespace Sample.WinForm;

class WinAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private static UserInfo current;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = GetPrincipal(current);
        var state = new AuthenticationState(principal);
        return Task.FromResult(state);
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