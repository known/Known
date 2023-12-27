using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class InstallPage : BaseForm<InstallInfo>
{
    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<InstallInfo>(UI)
        {
            LabelSpan = 6,
            Data = Context.Install
        };

        await base.OnInitFormAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", $"{Config.App.Name} - 安装");
            builder.Div("kui-install-body", () =>
            {
                builder.Div("kui-install-form", () =>
                {
                    base.BuildRenderTree(builder);
                    builder.Div("button", () =>
                    {
                        UI.Button(builder, "开始使用", this.Callback<MouseEventArgs>(OnStartAsync), "primary");
                    });
                });
            });
            builder.Div("kui-install-foot", () => builder.Component<PageFooter>().Build());
        });
    }

    private async void OnStartAsync(MouseEventArgs args)
    {
        //TODO：安装表单验证问题
        if (!Model.Validate())
            return;

        var result = await Platform.System.SaveInstallAsync(Model.Data);
        UI.Result(result, () =>
        {
            var info = result.DataAs<InstallInfo>();
            OnInstall?.Invoke(info);
        });
    }
}