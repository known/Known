namespace Known;

public class PlatformService(Context context)
{
    private FileService File { get; } = new FileService(context);
    private FlowService Flow { get; } = new FlowService(context);

    //User
    public Task<List<SysUser>> GetUsersByRoleAsync(Database db, string roleName) => UserRepository.GetUsersByRoleAsync(db, roleName);
    public Task SyncUserAsync(Database db, SysUser user) => UserService.SyncUserAsync(db, user);

    //File
    public Task<List<SysFile>> GetFilesAsync(string bizId) => File.GetFilesAsync(bizId);
    public void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);
    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFilesAsync(db, bizId, oldFiles);
    //public Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles) => File.SaveFileAsync(db, file, bizId, bizType, oldFiles);
    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => FileService.AddFilesAsync(db, files, bizId, bizType);

    //Flow
    public Task CreateFlowAsync(Database db, FlowBizInfo info) => Flow.CreateFlowAsync(db, info);
    public Task DeleteFlowAsync(Database db, string bizId) => FlowService.DeleteFlowAsync(db, bizId);
    public Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null) => FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);

    //Weixin
    public Task<SysWeixin> GetWeixinAsync(Database db, SysUser user) => WeixinRepository.GetWeixinByUserIdAsync(db, user.Id);
    public Task<Result> SendTemplateMessageAsync(TemplateInfo info) => WeixinService.SendTemplateMessageAsync(info);
}