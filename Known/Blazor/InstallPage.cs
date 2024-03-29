﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class InstallPage : BaseForm<InstallInfo>
{
    [Parameter] public RenderFragment TopMenu { get; set; }
    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<InstallInfo>(Context)
        {
            LabelSpan = 6,
            Data = Context.Install
        };

        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", () =>
            {
                builder.Div("", $"{Language["App.Name"]} - {Language["Install"]}");
                builder.Fragment(TopMenu);
            });
            builder.Div("kui-install-body", () =>
            {
                builder.Div("kui-install-form", () =>
                {
                    base.BuildForm(builder);
                    builder.Div("button", () =>
                    {
                        UI.Button(builder, Language["StartUsing"], this.Callback<MouseEventArgs>(OnStartAsync), "primary");
                    });
                });
            });
            builder.Div("kui-install-foot", () => builder.Component<PageFooter>().Build());
        });
    }

    private async void OnStartAsync(MouseEventArgs args)
    {
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