namespace Known.Services;

class SettingService : BaseService
{
    //Setting
    public Task<List<SysSetting>> GetSettingsAsync(string bizType) => SettingRepository.GetSettingsAsync(Database, bizType);
    public Task<SysSetting> GetSettingByCompAsync(string bizType) => PlatformHelper.GetSettingByCompAsync(Database, bizType);
    public Task<SysSetting> GetSettingByUserAsync(string bizType) => PlatformHelper.GetSettingByUserAsync(Database, bizType);

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

    public async Task<Result> SaveSettingAsync(dynamic model)
    {
        var entity = await Database.QueryByIdAsync<SysSetting>((string)model.Id);
        entity ??= new SysSetting();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(entity);
        }, entity);
    }
}