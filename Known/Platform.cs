namespace Known;

public class Platform
{
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