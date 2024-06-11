namespace Known;

public class PlatformService(Context context)
{
    internal AuthService Auth { get; } = new AuthService(context);
    internal ModuleService Module { get; } = new ModuleService(context);
    internal SystemService System { get; } = new SystemService(context);
    internal SettingService Setting { get; } = new SettingService(context);
    internal CompanyService Company { get; } = new CompanyService(context);
    internal DictionaryService Dictionary { get; } = new DictionaryService(context);
    internal FileService File { get; } = new FileService(context);
    internal FlowService Flow { get; } = new FlowService(context);
    internal RoleService Role { get; } = new RoleService(context);
    internal UserService User { get; } = new UserService(context);
    internal AutoService Auto { get; } = new AutoService(context);
    internal WeixinService Weixin { get; } = new WeixinService(context);

    public Context Context { get; } = context;

    #region Setting
    public Task<List<SysSetting>> GetSettingsAsync(string bizType) => Setting.GetSettingsAsync(bizType);
    public Task<T> GetSettingAsync<T>(string bizType) => Setting.GetSettingAsync<T>(bizType);
    public Task DeleteSettingAsync(Database db, string bizType) => Setting.DeleteSettingAsync(db, bizType);
    public Task SaveSettingAsync(Database db, string bizType, object bizData) => Setting.SaveSettingAsync(db, bizType, bizData);
    public Task<List<SysSetting>> GetUserSettingsAsync(string bizType) => Setting.GetUserSettingsAsync(bizType);
    public Task<T> GetUserSettingAsync<T>(string bizType) => Setting.GetUserSettingAsync<T>(bizType);
    public Task DeleteUserSettingAsync(Database db, string bizType) => Setting.DeleteUserSettingAsync(db, bizType);
    public Task SaveUserSettingAsync(Database db, string bizType, object bizData) => Setting.SaveUserSettingAsync(db, bizType, bizData);
    #endregion

    #region Company
    public Task<T> GetCompanyAsync<T>() => Company.GetCompanyAsync<T>();
    public Task<Result> SaveCompanyAsync(object model) => Company.SaveCompanyAsync(model);
    #endregion

    #region User
    public Task<Result> SignOutAsync(string token) => Auth.SignOutAsync(token);
    public Task<UserInfo> GetUserAsync(string userName) => Auth.GetUserAsync(userName);
    public Task<List<SysUser>> GetUsersByRoleAsync(string roleName) => User.GetUsersByRoleAsync(roleName);
    public Task SyncUserAsync(Database db, SysUser user) => User.SyncUserAsync(db, user);
    #endregion

    #region File
    public Task<List<SysFile>> GetFilesAsync(string bizId) => File.GetFilesAsync(bizId);
    public void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);
    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => File.DeleteFilesAsync(db, bizId, oldFiles);
    public Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles) => File.SaveFileAsync(db, file, bizId, bizType, oldFiles);
    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => File.AddFilesAsync(db, files, bizId, bizType);
    #endregion

    #region Flow
    public Task CreateFlowAsync(Database db, FlowBizInfo info) => Flow.CreateFlowAsync(db, info);
    public Task DeleteFlowAsync(Database db, string bizId) => Flow.DeleteFlowAsync(db, bizId);
    public Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null) => Flow.AddFlowLogAsync(db, bizId, stepName, result, note, time);
    public Task<List<SysFlowLog>> GetFlowLogsAsync(string bizId) => Flow.GetFlowLogsAsync(bizId);
    #endregion

    #region Weixin
    public Task<string> GetWeixinQRCodeUrlAsync(string sceneId) => Weixin.GetQRCodeUrlAsync(sceneId);
    public Task<WeixinInfo> GetWeixinAsync() => Weixin.GetWeixinAsync();
    public Task<SysWeixin> GetWeixinAsync(UserInfo user) => Weixin.GetWeixinAsync(user);
    public Task<SysWeixin> GetWeixinAsync(Database db, SysUser user) => Weixin.GetWeixinAsync(db, user);
    public Task<Result> SendTemplateMessageAsync(TemplateInfo info) => Weixin.SendTemplateMessageAsync(info);
    #endregion
}