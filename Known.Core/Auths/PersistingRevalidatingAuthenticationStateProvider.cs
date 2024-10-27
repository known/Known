using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Core.Auths
{
    // This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
    // every 30 minutes an interactive circuit is connected. It also uses PersistentComponentState to flow the
    // authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
    internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider, IAuthStateProvider
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly PersistentComponentState state;
        private readonly IdentityOptions options;

        private readonly PersistingComponentStateSubscription subscription;

        private Task<AuthenticationState>? authenticationStateTask;

        public PersistingRevalidatingAuthenticationStateProvider(
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
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }

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
            if (user.User.Identity.IsAuthenticated)
            {
                return new UserInfo
                {
                    Id = user.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserName = user.User.FindFirstValue(ClaimTypes.Name),
                    Role = user.User.FindFirstValue(ClaimTypes.Role),
                };
            }

            return null;
        }

        public Task SetUserAsync(Known.UserInfo user)
        {
            return Task.CompletedTask;
        }
    }
}
