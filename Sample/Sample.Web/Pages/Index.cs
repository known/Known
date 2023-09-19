namespace Sample.Web.Pages;

[Route("/")]
public class Index : Razor.Index
{
    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

    protected override async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthState == null)
            return null;

        var state = await AuthState;
        var isLogin = state != null && state.User != null && state.User.Identity != null && state.User.Identity.IsAuthenticated;
        if (!isLogin)
            return null;

        var userName = state?.User?.Identity?.Name;
        return await Platform.User.GetUserAsync(userName);
    }
}