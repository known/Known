using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sample.Web.Auths;

internal sealed class PersistingStateProvider : RevalidatingServerAuthenticationStateProvider, IAuthStateProvider
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

    public async Task<UserInfo> GetUserAsync()
    {
        if (authenticationStateTask == null)
            return null;

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;
        if (principal.Identity?.IsAuthenticated == false)
            return null;

        await using var scope = scopeFactory.CreateAsyncScope();
        var platform = scope.ServiceProvider.GetRequiredService<PlatformService>();
        return await platform.GetUserAsync(principal.Identity.Name);
    }

    public Task SetUserAsync(UserInfo user)
    {
        var principal = GetPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        return Task.CompletedTask;
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

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal();
    }

    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}