namespace Known.Services;

public interface ISettingService : IService
{
    Task<string> GetUserSettingAsync(string bizType);
    Task<Result> DeleteUserSettingAsync(string bizType);
    Task<Result> SaveUserSettingAsync(SettingInfo info);
    Task<Result> SaveUserSettingAsync(SettingFormInfo info);
}

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
        var setting = await GetUserSettingAsync(Database, bizType);
        if (setting != null)
            await Database.DeleteAsync(setting);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task<Result> SaveUserSettingAsync(SettingInfo info)
    {
        return SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = SettingInfo.KeyInfo,
            BizData = info
        });
    }

    public async Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        var setting = await GetUserSettingAsync(Database, info.BizType);
        setting ??= new SysSetting();
        setting.BizType = info.BizType;
        setting.BizData = Utils.ToJson(info.BizData);
        await Database.SaveAsync(setting);
        return Result.Success(Language.Success(Language.Save));
    }

    internal static async Task<T> GetUserSettingAsync<T>(Database db, string bizType)
    {
        var setting = await GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    private static Task<SysSetting> GetUserSettingAsync(Database db, string bizType)
    {
        return db.QueryAsync<SysSetting>(d => d.CreateBy == db.User.UserName && d.BizType == bizType);
    }
}