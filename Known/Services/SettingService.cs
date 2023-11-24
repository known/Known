using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class SettingService : ServiceBase
{
    //Setting
    internal Task<List<SysSetting>> GetSettingsAsync(string bizType) => SettingRepository.GetSettingsAsync(Database, bizType);

    internal async Task<T> GetSettingAsync<T>(string bizType)
    {
        var setting = await SettingRepository.GetSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal async Task DeleteSettingAsync(Database db, string bizType)
    {
        var setting = await SettingRepository.GetSettingAsync(db, bizType);
        if (setting == null)
            return;

        await db.DeleteAsync(setting);
    }

    internal async Task SaveSettingAsync(Database db, string bizType, object bizData)
    {
        var setting = await SettingRepository.GetSettingAsync(db, bizType);
        setting ??= new SysSetting();
        setting.BizType = bizType;
        setting.BizData = Utils.ToJson(bizData);
        await db.SaveAsync(setting);
    }

    internal async Task<Result> SaveSettingAsync(string bizType, object bizData)
    {
        await SaveSettingAsync(Database, bizType, bizData);
        return Result.Success("保存成功！");
    }

    internal Task<List<SysSetting>> GetUserSettingsAsync(string bizType) => SettingRepository.GetUserSettingsAsync(Database, bizType);

    internal async Task<T> GetUserSettingAsync<T>(string bizType)
    {
        var setting = await SettingRepository.GetUserSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal async Task DeleteUserSettingAsync(Database db, string bizType)
    {
        var setting = await SettingRepository.GetUserSettingAsync(db, bizType);
        if (setting == null)
            return;

        await db.DeleteAsync(setting);
    }

    internal async Task<Result> DeleteUserSettingAsync(string bizType)
    {
        await DeleteUserSettingAsync(Database, bizType);
        return Result.Success("删除成功！");
    }

    public async Task<Result> DeleteSettingsAsync(List<SysSetting> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveSettingAsync(SysSetting model)
    {
        var vr = model.Validate();
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
        }, model);
    }
}