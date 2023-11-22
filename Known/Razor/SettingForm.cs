using Known.Entities;

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
        var setting = await Platform.Setting.GetSettingByUserAsync(SettingInfo.KeyInfo);
        setting ??= new SysSetting { BizType = SettingInfo.KeyInfo, BizName = "系统设置" };
        setting.BizData = Utils.ToJson(Model);
        var result = await Platform.Setting.SaveSettingAsync(setting);
        if (result.IsValid)
        {
            Context.UserSetting = Model;
            Context.RefreshPage();
        }
    }

    protected async Task ResetAsync()
    {
        var setting = await Platform.Setting.GetSettingByUserAsync(SettingInfo.KeyInfo);
        var result = await Platform.Setting.DeleteSettingsAsync([setting]);
        if (result.IsValid)
        {
            Model = new();
            Context.UserSetting = Model;
            Context.RefreshPage();
        }
    }
}