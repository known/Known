namespace Known;

public class Platform
{
    private static readonly Dictionary<string, Type> dbTypes = [];

    public static Type RepositoryType { get; set; }
    public static void RegisterDatabase(Type type, string name = "Default") => dbTypes[name] = type;

    internal static Database CreateDatabase(string name = "Default")
    {
        if (!dbTypes.TryGetValue(name, out Type type))
            return new DefaultDatabase(name);

        if (Activator.CreateInstance(type) is not Database instance)
            throw new SystemException($"The {type} is not implement Database");

        return instance;
    }

    internal static IDataRepository CreateRepository()
    {
        if (RepositoryType == null)
            return new DataRepository();

        if (Activator.CreateInstance(RepositoryType) is not IDataRepository instance)
            throw new SystemException($"The {RepositoryType} is not implement IDatabase");

        return instance;
    }

    //System
    public static Task<SystemInfo> GetSystemAsync(Database db) => SystemService.GetSystemAsync(db);

    //User
    public static Task<List<SysUser>> GetUsersByRoleAsync(Database db, string roleName) => UserService.GetUsersByRoleAsync(db, roleName);
    public static Task SyncUserAsync(Database db, SysUser user) => UserService.SyncUserAsync(db, user);

    //File
    public static Task<List<SysFile>> GetFilesAsync(Database db, string bizId) => FileService.GetFilesAsync(db, bizId);
    public static void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);
    public static Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFilesAsync(db, bizId, oldFiles);
    public static Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => FileService.AddFilesAsync(db, files, bizId, bizType);

    //Flow
    public static Task CreateFlowAsync(Database db, FlowBizInfo info) => FlowService.CreateFlowAsync(db, info);
    public static Task DeleteFlowAsync(Database db, string bizId) => FlowService.DeleteFlowAsync(db, bizId);
    public static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null) => FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);

    //Weixin
    public static Task<SysWeixin> GetWeixinAsync(Database db, SysUser user) => WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    public static Task<Result> SendTemplateMessageAsync(TemplateInfo info) => WeixinApi.SendTemplateMessageAsync(info);
}