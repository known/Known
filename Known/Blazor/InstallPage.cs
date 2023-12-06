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
        builder.Div("install", () =>
        {
            builder.Markup($"<h1>欢迎使用{Config.App.Id}</h1>");
            UI.BuildForm(builder, model);
            builder.Div("button", () =>
            {
                UI.Button(builder, "开始使用", Callback<MouseEventArgs>(OnStart), "primary");
            });
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