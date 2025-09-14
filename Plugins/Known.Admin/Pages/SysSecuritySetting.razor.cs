namespace Known.Pages;

public partial class SysSecuritySetting
{
    private SystemInfo Model = new();
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Model = Parent.Model.System ?? new();
        Model.MaxFileSize ??= Config.App.UploadMaxSize;
    }

    private async Task OnSaveDefaultPwdAsync(string value)
    {
        Model.UserDefaultPwd = value;
        await Parent.SaveSystemAsync(Model);
    }

    private async Task OnLoginCaptchaChangedAsync(bool value)
    {
        Model.IsLoginCaptcha = value;
        await Parent.SaveSystemAsync(Model);
    }

    private async Task OnWatermarkChangedAsync(bool value)
    {
        Model.IsWatermark = value;
        await Parent.SaveSystemAsync(Model);
    }

    private async Task OnMaxFileSizeChangedAsync(int? value)
    {
        Model.MaxFileSize = value;
        await Parent.SaveSystemAsync(Model);
    }
}