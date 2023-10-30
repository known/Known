namespace Known.Helpers;

public sealed class PlatformHelper
{
    private PlatformHelper() { }

    public static Action<Database, SysOrganization> Organization { get; set; }
    public static Action<Database, UserInfo> User { get; set; }
    public static Func<Database, List<CodeInfo>> UserDatas { get; set; }
    public static Func<Database, List<CodeInfo>> Dictionary { get; set; }
    public static Func<Database, SystemInfo, Result> CheckSystem { get; set; }
    public static Func<Database, SysUser, Result> CheckUser { get; set; }
    public static Func<InstallInfo, Task<Result>> UpdateKeyAsync { get; set; }

    //Setting
    public static async Task<SysSetting> GetSettingByCompAsync(Database db, string bizType) => await SettingRepository.GetSettingByCompAsync(db, bizType) ?? new SysSetting { BizType = bizType };
    public static async Task<T> GetSettingByCompAsync<T>(Database db, string bizType)
    {
        var setting = await GetSettingByCompAsync(db, bizType);
        return setting.DataAs<T>();
    }

    public static async Task<List<SysSetting>> GetSettingsByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingsByUserAsync(db, bizType);
    public static async Task<SysSetting> GetSettingByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingByUserAsync(db, bizType) ?? new SysSetting { BizType = bizType };
    public static async Task<T> GetSettingByUserAsync<T>(Database db, string bizType)
    {
        var setting = await GetSettingByUserAsync(db, bizType);
        return setting.DataAs<T>();
    }

    internal static void SetBizOrganization(Database db, SysOrganization entity)
    {
        Organization?.Invoke(db, new SysOrganization
        {
            CompNo = db.User.CompNo,
            Code = entity.Code,
            Name = entity.Name,
            Note = entity.Note
        });
    }

    internal static void SetBizUser(Database db, SysUser entity)
    {
        var info = Utils.MapTo<UserInfo>(entity);
        User?.Invoke(db, info);
    }
}