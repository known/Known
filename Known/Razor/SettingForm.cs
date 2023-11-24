namespace Known.Razor;

public class SettingForm : BaseComponent
{
    protected SettingInfo Model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model = Context.UserSetting;
    }

    protected async Task SaveAsync()
    {
        var result = await Platform.Setting.SaveSettingAsync(SettingInfo.KeyInfo, Model);
        if (result.IsValid)
        {
            Context.UserSetting = Model;
            Context.RefreshPage();
        }
    }

    protected async Task ResetAsync()
    {
        var result = await Platform.Setting.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            Model = new();
            Context.UserSetting = Model;
            Context.RefreshPage();
        }
    }
}