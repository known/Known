namespace Known.Services;

public interface ISettingService : IService
{
    Task<string> GetUserSettingAsync(string bizType);
    Task<Result> DeleteUserSettingAsync(string bizType);
    Task<Result> SaveUserSettingAsync(string bizType, object bizData);
}

class SettingService(Context context) : ServiceBase(context), ISettingService
{
    //Setting
    //internal Task<List<SysSetting>> GetSettingsAsync(string bizType) => SettingRepository.GetSettingsAsync(Database, bizType);

    //internal async Task<T> GetSettingAsync<T>(string bizType)
    //{
    //    var setting = await SettingRepository.GetSettingAsync(Database, bizType);
    //    if (setting == null)
    //        return default;

    //    return setting.DataAs<T>();
    //}

    //internal static async Task DeleteSettingAsync(Database db, string bizType)
    //{
    //    var setting = await SettingRepository.GetSettingAsync(db, bizType);
    //    if (setting == null)
    //        return;

    //    await db.DeleteAsync(setting);
    //}

    //internal static async Task SaveSettingAsync(Database db, string bizType, object bizData)
    //{
    //    var setting = await SettingRepository.GetSettingAsync(db, bizType);
    //    setting ??= new SysSetting();
    //    setting.BizType = bizType;
    //    setting.BizData = Utils.ToJson(bizData);
    //    await db.SaveAsync(setting);
    //}

    //internal async Task<Result> SaveSettingAsync(string bizType, object bizData)
    //{
    //    await SaveSettingAsync(Database, bizType, bizData);
    //    return Result.Success(Language.Success(Language.Save));
    //}

    //internal Task<List<SysSetting>> GetUserSettingsAsync(string bizType) => SettingRepository.GetUserSettingsAsync(Database, bizType);

    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await SettingRepository.GetUserSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }
    
    internal static async Task<T> GetUserSettingAsync<T>(Database db, string bizType)
    {
        var setting = await SettingRepository.GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal static async Task DeleteUserSettingAsync(Database db, string bizType)
    {
        var setting = await SettingRepository.GetUserSettingAsync(db, bizType);
        if (setting == null)
            return;

        await db.DeleteAsync(setting);
    }

    public async Task<Result> DeleteUserSettingAsync(string bizType)
    {
        await DeleteUserSettingAsync(Database, bizType);
        return Result.Success(Language.Success(Language.Delete));
    }

    internal static async Task SaveUserSettingAsync(Database db, string bizType, object bizData)
    {
        var setting = await SettingRepository.GetUserSettingAsync(db, bizType);
        setting ??= new SysSetting();
        setting.BizType = bizType;
        setting.BizData = Utils.ToJson(bizData);
        await db.SaveAsync(setting);
    }

    public async Task<Result> SaveUserSettingAsync(string bizType, object bizData)
    {
        await SaveUserSettingAsync(Database, bizType, bizData);
        return Result.Success(Language.Success(Language.Save));
    }
}