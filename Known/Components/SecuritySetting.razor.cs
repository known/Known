namespace Known.Components;

/// <summary>
/// 安全设置组件。
/// </summary>
public partial class SecuritySetting
{
    private SystemInfo Model = new();

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Model = await Admin.GetSystemAsync();
            Model.MaxFileSize ??= Config.App.UploadMaxSize;
            Model.PwdComplexity ??= nameof(PasswordComplexity.None);
            StateChanged();
        }
    }

    private async Task OnSaveModel()
    {
        var result = await Admin.SaveSystemAsync(Model);
        UI.Result(result);
    }
}