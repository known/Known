namespace Known.Razor;

public class SysSystem : BasePage
{
    protected SystemInfo Model { get; private set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Model = await Platform.System.GetSystemAsync();
    }

    protected async void OnSaveAppName(string value)
    {
        Model.AppName = value;
        var result = await Platform.System.SaveSystemAsync(Model);
        if (result.IsValid)
        {
            CurrentUser.AppName = value;
            Context.RefreshPage();
        }
    }

    protected async void OnSaveProductKey(string value)
    {
        Model.ProductKey = value;
        await Platform.System.SaveSystemAsync(Model);
    }

    protected async void OnSaveDefaultPwd(string value)
    {
        Model.UserDefaultPwd = value;
        await Platform.System.SaveSystemAsync(Model);
    }
}