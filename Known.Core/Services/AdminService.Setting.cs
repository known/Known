namespace Known.Services;

partial class AdminService
{
    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await Database.GetUserSettingAsync(bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        var database = Database;
        var setting = await database.GetUserSettingAsync(info.BizType);
        if (setting != null && info.BizData == null)
        {
            await database.DeleteAsync<SysSetting>(setting.Id);
        }
        else
        {
            setting ??= new SettingInfo();
            setting.BizType = info.BizType;
            setting.BizData = Utils.ToJson(info.BizData);
            await database.SaveSettingAsync(setting);
        }
        return Result.Success(Language.SaveSuccess);
    }
}