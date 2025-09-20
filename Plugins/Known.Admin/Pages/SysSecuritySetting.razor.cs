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
        Model.PwdComplexity ??= nameof(PasswordComplexity.None);
    }

    private async Task OnSaveModel()
    {
        var result = await Parent.SaveSystemAsync(Model);
        UI.Result(result);
    }
}