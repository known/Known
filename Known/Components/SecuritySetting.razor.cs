namespace Known.Components;

/// <summary>
/// 安全设置组件。
/// </summary>
public partial class SecuritySetting
{
    private ISystemService Service;
    private SystemInfo Model = new();

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Model = await Service.GetSystemAsync();
            Model.MaxFileSize ??= Config.App.UploadMaxSize;
            Model.PwdComplexity ??= nameof(PasswordComplexity.None);
            StateChanged();
        }
    }

    private async Task OnSaveModel()
    {
        var result = await Service.SaveSystemAsync(Model);
        UI.Result(result);
    }
}