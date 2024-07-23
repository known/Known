namespace Known.Components;

class SysActive : BaseComponent
{
    private ISystemService systemService;
    private FormModel<SystemInfo> model;

    [Parameter] public Action<bool> OnCheck { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        systemService = await CreateServiceAsync<ISystemService>();
        model = new FormModel<SystemInfo>(this);
        model.AddRow().AddColumn(c => c.ProductId);
        model.AddRow().AddColumn(c => c.ProductKey);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            model.Data = await systemService.GetSystemAsync();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildResult(builder, "403", Config.AuthStatus);
        builder.Div("kui-form-auth", () =>
        {
            UI.BuildForm(builder, model);
            builder.FormPageButton(() =>
            {
                builder.Button(new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnAuthAsync));
            });
        });
    }

    private async void OnAuthAsync(MouseEventArgs args)
    {
        if (!model.Validate())
            return;

        var result = await systemService.SaveKeyAsync(model.Data);
        UI.Result(result, () =>
        {
            OnCheck?.Invoke(result.IsValid);
            return Task.CompletedTask;
        });
    }
}