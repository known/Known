using Microsoft.AspNetCore.Components;

namespace Known.Server;

public partial class App
{
    [Inject] private UIContext Context { get; set; }
    [CascadingParameter] private HttpContext HttpContext { get; set; }
    //private InteractiveServerRenderMode InteractiveMode => HttpContext.Request.Path.StartsWithSegments("/login1") ? null : new(false);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Context.IPAddress = HttpContext.Connection?.RemoteIpAddress?.ToString();
        Context.IsMobile = HttpContext.CheckMobile();
        Config.HostUrl = HttpContext.GetHostUrl();
    }
}