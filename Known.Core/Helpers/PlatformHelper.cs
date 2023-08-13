namespace Known.Core.Helpers;

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
    public static SysSetting GetSettingByComp(Database db, string bizType) => SettingRepository.GetSettingByComp(db, bizType) ?? new SysSetting { BizType = bizType };
    public static T GetSettingByComp<T>(Database db, string bizType) => GetSettingByComp(db, bizType).DataAs<T>();
    public static List<SysSetting> GetSettingsByUser(Database db, string bizType) => SettingRepository.GetSettingsByUser(db, bizType);
    public static SysSetting GetSettingByUser(Database db, string bizType) => SettingRepository.GetSettingByUser(db, bizType) ?? new SysSetting { BizType = bizType };
    public static T GetSettingByUser<T>(Database db, string bizType) => GetSettingByUser(db, bizType).DataAs<T>();
    //Company
    public static string GetCompany(Database db, UserInfo user) => CompanyService.GetCompany(db, user);
    public static SysTenant GetTenant(Database db, string compNo) => SystemRepository.GetTenant(db, compNo);
    //File
    public static void DeleteFiles(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFiles(db, bizId, oldFiles);
    public static SysFile SaveFile(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles, bool isThumb = false) => FileService.SaveFile(db, file, bizId, bizType, oldFiles, isThumb);
    public static List<SysFile> AddFiles(Database db, List<AttachFile> files, string bizId, string bizType, bool isThumb = false) => FileService.AddFiles(db, files, bizId, bizType, isThumb);
    //Flow
    public static void CreateFlow(Database db, FlowBizInfo info)
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
        db.Save(flow);
        AddFlowLog(db, info.BizId, stepName, "创建", info.BizName);
    }

    public static void DeleteFlow(Database db, string bizId)
    {
        FlowRepository.DeleteFlowLogs(db, bizId);
        FlowRepository.DeleteFlow(db, bizId);
    }

    public static void AddFlowLog(Database db, string bizId, string stepName, string result, string note) => FlowService.AddFlowLog(db, bizId, stepName, result, note);

    internal static string GetProductId()
    {
        var mac = Platform.GetMacAddress();
        var id = mac.Split(':').Select(m => Convert.ToInt32(m, 16)).Sum();
        return $"PM-{Config.AppId}-{id:000000}";
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