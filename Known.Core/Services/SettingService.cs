namespace Known.Core.Services;

class SettingService : BaseService
{
    internal SettingService(Context context) : base(context) { }

    //Setting
    internal List<SysSetting> GetSettings(string bizType) => SettingRepository.GetSettings(Database, bizType);
    internal SysSetting GetSettingByComp(string bizType) => PlatformHelper.GetSettingByComp(Database, bizType);
    internal SysSetting GetSettingByUser(string bizType) => PlatformHelper.GetSettingByUser(Database, bizType);
    
    internal Result DeleteSettings(List<SysSetting> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                db.Delete(item);
            }
        });
    }

    internal Result SaveSetting(dynamic model)
    {
        var entity = Database.QueryById<SysSetting>((string)model.Id);
        entity ??= new SysSetting();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
        }, entity);
    }
}