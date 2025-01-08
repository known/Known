﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Known.Auths;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 30 minutes an interactive circuit is connected. It also uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
sealed class IdentityAuthStateProvider : RevalidatingServerAuthenticationStateProvider, IAuthStateProvider
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IAdminService platform;
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState> authenticationStateTask;

    public IdentityAuthStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        IHttpContextAccessor httpContextAccessor,
        IAdminService platformService,
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        scopeFactory = serviceScopeFactory;
        contextAccessor = httpContextAccessor;
        platform = platformService;
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserInfo>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<UserInfo> userManager, ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
            return false;

        if (!userManager.SupportsUserSecurityStamp)
            return true;

        var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
        var userStamp = await userManager.GetSecurityStampAsync(user);
        return principalStamp == userStamp;
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
                    Id = userId,
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

    public async Task<UserInfo> GetUserAsync()
    {
        var user = await GetAuthenticationStateAsync();
        if (!user.User.Identity.IsAuthenticated)
            return null;

        var userName = user.User.FindFirstValue(ClaimTypes.Name);
        return await AuthHelper.GetUserAsync(platform, userName);
    }

    public Task SignInAsync(UserInfo user)
    {
        if (user == null)
            return Task.CompletedTask;

        var principal = user.ToPrincipal(IdentityConstants.ApplicationScheme);
        return contextAccessor.HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme, principal,
            new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(1) }
        );
    }

    public Task SignOutAsync()
    {
        return contextAccessor.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }
}