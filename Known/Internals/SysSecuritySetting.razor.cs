namespace Known.Internals;

/// <summary>
/// 系统安全设置页面标签组件类。
/// </summary>
public partial class SysSecuritySetting
{
    private SystemInfo Model = new();
    [CascadingParameter] private SysSystem Parent { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Model = Parent.Model.System ?? new();
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
}