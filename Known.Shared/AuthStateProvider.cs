using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Known;

internal sealed class PersistingStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState> authenticationStateTask;

    public PersistingStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        scopeFactory = serviceScopeFactory;
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var platform = scope.ServiceProvider.GetRequiredService<PlatformService>();
        return await ValidateSecurityStampAsync(platform, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(PlatformService platform, ClaimsPrincipal principal)
    {
        var user = await platform.GetUserAsync(principal.Identity.Name);
        if (user is null)
            return false;

        //if (!userManager.SupportsUserSecurityStamp)
        //    return true;

        //var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
        //var userStamp = await userManager.GetSecurityStampAsync(user);
        //return principalStamp == userStamp;
        return true;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;

            if (userId != null && email != null)
            {
                state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserName = userId,
                    Email = email,
                });
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}

class WebAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private const string KeyUser = "Known_User";
    private readonly ProtectedSessionStorage sessionStorage;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public WebAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        this.sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = new AuthenticationState(anonymous);
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        var user = result.Success ? result.Value : null;
        if (user != null)
        {
            var principal = GetPrincipal(user);
            state = new AuthenticationState(principal);
        }
        return state;
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var result = await sessionStorage.GetAsync<UserInfo>(KeyUser);
        return result.Value;
    }

    public async Task UpdateUserAsync(UserInfo user)
    {
        if (user == null)
        {
            await sessionStorage.DeleteAsync(KeyUser);
            return;
        }

        await sessionStorage.SetAsync(KeyUser, user);
        var principal = GetPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        }, "Known_Auth"));
    }
}

class WinAuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    private static UserInfo current;
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = new AuthenticationState(anonymous);
        if (current != null)
        {
            var principal = GetPrincipal(current);
            state = new AuthenticationState(principal);
        }
        return Task.FromResult(state);
    }

    public Task<UserInfo> GetUserAsync() => Task.FromResult(current);

    public Task UpdateUserAsync(UserInfo user)
    {
        var principal = anonymous;
        current = user;

        if (user != null)
            principal = GetPrincipal(user);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        }, "Known_Auth"));
    }
}