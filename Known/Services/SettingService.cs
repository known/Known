using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class SettingService : ServiceBase
{
    //Setting
    public Task<List<SysSetting>> GetSettingsAsync(string bizType) => SettingRepository.GetSettingsAsync(Database, bizType);
    public Task<SysSetting> GetSettingByCompAsync(string bizType) => SettingRepository.GetSettingByCompAsync(Database, bizType);
    public Task<SysSetting> GetSettingByUserAsync(string bizType) => SettingRepository.GetSettingByUserAsync(Database, bizType);

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