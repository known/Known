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
        model = new(UI);
        model.LabelSpan = 6;
        model.Data = Context.Install;
        model.AddRow().AddColumn(c => c.CompNo);
        model.AddRow().AddColumn(c => c.CompName);
        model.AddRow().AddColumn(c => c.AppName);
        model.AddRow().AddColumn(c => c.UserName);
        model.AddRow().AddColumn(c => c.Password);
        model.AddRow().AddColumn(c => c.Password1);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("install", () =>
        {
            builder.Markup($"<h1>欢迎使用{Config.App.Id}</h1>");
            UI.BuildForm(builder, model);
            builder.Div("button", () =>
            {
                UI.BuildButton(builder, new ActionInfo
                {
                    Style = "primary",
                    Name = "开始使用",
                    OnClick = Callback<MouseEventArgs>(OnStart)
                });
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