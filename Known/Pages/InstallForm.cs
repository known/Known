namespace Known.Pages;

public class InstallForm : BaseForm<InstallInfo>
{
    private ISystemService systemService;

    [Parameter] public RenderFragment TopMenu { get; set; }
    [Parameter] public Action<InstallInfo> OnInstall { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        systemService = await CreateServiceAsync<ISystemService>();
        Model = new FormModel<InstallInfo>(Context, true) { LabelSpan = 6 };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            Model.Data = await systemService.GetInstallAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-install", () =>
        {
            builder.Div("kui-install-head", () =>
            {
                builder.Div("", $"{Config.App.Name} - {Language["Install"]}");
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

        var result = await systemService.SaveInstallAsync(Model.Data);
        UI.Result(result, () =>
        {
            var info = result.DataAs<InstallInfo>();
            OnInstall?.Invoke(info);
            return Task.CompletedTask;
        });
    }
}