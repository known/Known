using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Razor;

public class InstallPage : BaseComponent
{
    protected InstallInfo Model = new();

    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override void OnInitialized()
    {
        Model = Context.Install;
    }

    protected async Task OnStart(EditContext context)
    {
        var result = await Platform.System.SaveInstallAsync(Model);
        UI.Result(result, () =>
        {
            var info = result.DataAs<InstallInfo>();
            OnInstall?.Invoke(info);
        });
    }
}