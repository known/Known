namespace Known.Core.Services;

class SettingService(Context context) : ServiceBase(context), ISettingService
{
    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await GetUserSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> DeleteUserSettingAsync(string bizType)
    {
        var database = Database;
        var setting = await GetUserSettingAsync(database, bizType);
        if (setting != null)
            await database.DeleteAsync(setting);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task<Result> SaveUserSettingInfoAsync(SettingInfo info)
    {
        return SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Constant.UserSetting,
            BizData = info
        });
    }

    public async Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        var database = Database;
        var setting = await GetUserSettingAsync(database, info.BizType);
        setting ??= new SysSetting();
        setting.BizType = info.BizType;
        setting.BizData = Utils.ToJson(info.BizData);
        await database.SaveAsync(setting);
        return Result.Success(Language.Success(Language.Save));
    }

    internal static async Task<T> GetUserSettingAsync<T>(Database db, string bizType)
    {
        var setting = await GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal static async Task<Dictionary<string, List<TableSettingInfo>>> GetUserTableSettingsAsync(Database db)
    {
        var settings = await db.QueryListAsync<SysSetting>(d => d.CreateBy == db.UserName && d.BizType.StartsWith("UserTable_"));
        if (settings == null || settings.Count == 0)
            return [];

        return settings.ToDictionary(k => k.BizType, v => v.DataAs<List<TableSettingInfo>>());
    }

    private static Task<SysSetting> GetUserSettingAsync(Database db, string bizType)
    {
        return db.QueryAsync<SysSetting>(d => d.CreateBy == db.UserName && d.BizType == bizType);
    }
}