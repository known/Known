using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class InstallPage : BaseComponent
{
    private FormModel<InstallInfo> model;

    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override void OnInitialized()
    {
        model = new FormModel<InstallInfo>(UI)
        {
            LabelSpan = 6,
            Data = Context.Install
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", $"{Config.App.Name}");
            builder.Div("kui-install-body", () =>
            {
                builder.Div("kui-install-form", () =>
                {
                    UI.BuildForm(builder, model);
                    builder.Div("button", () =>
                    {
                        UI.Button(builder, "开始使用", this.Callback<MouseEventArgs>(OnStart), "primary");
                    });
                });
            });
            builder.Div("kui-install-foot", () => builder.Component<PageFooter>().Build());
        });
    }

    private async void OnStart(MouseEventArgs args)
    {
        if (!model.Validate())
            return;

        var result = await Platform.System.SaveInstallAsync(model.Data);
        UI.Result(result, () =>
        {
            var info = result.DataAs<InstallInfo>();
            OnInstall?.Invoke(info);
        });
    }
}