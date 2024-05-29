using System.Security.Claims;

namespace Known.Shared.Auths;

class WebAuthStateProvider(ProtectedSessionStorage sessionStorage) : AuthenticationStateProvider, IAuthStateProvider
{
    private const string KeyUser = "Known_User";
    private readonly ProtectedSessionStorage sessionStorage = sessionStorage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        var user = result.Success ? result.Value : null;
        var principal = GetPrincipal(user);
        return new AuthenticationState(principal);
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        return result.Value;
    }

    public async Task SetUserAsync(UserInfo user)
    {
        if (user == null)
            await sessionStorage.DeleteAsync(KeyUser);
        else
            await sessionStorage.SetAsync(KeyUser, user);
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