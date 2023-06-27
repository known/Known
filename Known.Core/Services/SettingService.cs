namespace Known.Core.Services;

public class SettingService : BaseService
{
    internal SettingService(Context context) : base(context) { }

    //Public
    public static SysTenant GetTenant(Database db, string compNo) => SystemRepository.GetTenant(db, compNo);
    public static SysSetting GetSettingByComp(Database db, string bizType) => SettingRepository.GetSettingByComp(db, bizType) ?? new SysSetting { BizType = bizType };
    public static T GetSettingByComp<T>(Database db, string bizType) => GetSettingByComp(db, bizType).DataAs<T>();
    public static SysSetting GetSettingByUser(Database db, string bizType) => SettingRepository.GetSettingByUser(db, bizType) ?? new SysSetting { BizType = bizType };
    public static T GetSettingByUser<T>(Database db, string bizType) => GetSettingByUser(db, bizType).DataAs<T>();
    
    //Setting
    internal List<SysSetting> GetSettings(string bizType) => SettingRepository.GetSettings(Database, bizType);
    internal SysSetting GetSettingByComp(string bizType) => GetSettingByComp(Database, bizType);
    internal SysSetting GetSettingByUser(string bizType) => GetSettingByUser(Database, bizType);
    
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