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
    public static Func<InstallInfo, Result> UpdateKey { get; set; }

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

    //Company
    public static Task<string> GetCompanyAsync(Database db, UserInfo user) => CompanyService.GetCompanyAsync(db, user);
    //File
    public static Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFilesAsync(db, bizId, oldFiles);
    public static Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles, bool isThumb = false) => FileService.SaveFileAsync(db, file, bizId, bizType, oldFiles, isThumb);
    public static Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType, bool isThumb = false) => FileService.AddFilesAsync(db, files, bizId, bizType, isThumb);
    //Flow
    public static async Task CreateFlowAsync(Database db, FlowBizInfo info)
    {
        var stepName = "创建流程";
        var flow = new SysFlow
        {
            Id = Utils.GetGuid(),
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            FlowCode = info.FlowCode,
            FlowName = info.FlowName,
            FlowStatus = FlowStatus.Open,
            BizId = info.BizId,
            BizName = info.BizName,
            BizUrl = info.BizUrl,
            BizStatus = info.BizStatus,
            CurrStep = stepName,
            CurrBy = db.User.UserName
        };
        await db.SaveAsync(flow);
        await AddFlowLogAsync(db, info.BizId, stepName, "创建", info.BizName);
    }

    public static async Task DeleteFlowAsync(Database db, string bizId)
    {
        await FlowRepository.DeleteFlowLogsAsync(db, bizId);
        await FlowRepository.DeleteFlowAsync(db, bizId);
    }

    public static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note) => FlowService.AddFlowLogAsync(db, bizId, stepName, result, note);

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