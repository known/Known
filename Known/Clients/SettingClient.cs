namespace Known.Clients;

public class SettingClient : BaseClient
{
    public SettingClient(Context context) : base(context) { }

    public Task<List<SysSetting>> GetSettingsAsync(string bizType) => Context.GetAsync<List<SysSetting>>($"Setting/GetSettings?bizType={bizType}");
    public Task<SysSetting> GetSettingByCompAsync(string bizType) => Context.GetAsync<SysSetting>($"Setting/GetSettingByComp?bizType={bizType}");
    public Task<SysSetting> GetSettingByUserAsync(string bizType) => Context.GetAsync<SysSetting>($"Setting/GetSettingByUser?bizType={bizType}");
    public Task<Result> DeleteSettingsAsync(List<SysSetting> models) => Context.PostAsync("Setting/DeleteSettings", models);
    public Task<Result> SaveSettingAsync(object model) => Context.PostAsync("Setting/SaveSetting", model);
}