using Known.Entities;
using Known.Services;
using Known.WorkFlows;

namespace Known;

public class PlatformService(Context context)
{
    internal ModuleService Module { get; } = new ModuleService(context);
    internal SystemService System { get; } = new SystemService(context);
    internal SettingService Setting { get; } = new SettingService(context);
    internal CompanyService Company { get; } = new CompanyService(context);
    internal DictionaryService Dictionary { get; } = new DictionaryService(context);
    internal FileService File { get; } = new FileService(context);
    internal FlowService Flow { get; } = new FlowService(context);
    internal RoleService Role { get; } = new RoleService(context);
    internal UserService User { get; } = new UserService(context);
    internal AuthService Auth { get; } = new AuthService(context);
    internal AutoService Auto { get; } = new AutoService(context);

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
    public Task<UserInfo> GetUserAsync(string userName) => Auth.GetUserAsync(userName);
    #endregion

    #region File
    public void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);
    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => File.DeleteFilesAsync(db, bizId, oldFiles);
    public Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles) => File.SaveFileAsync(db, file, bizId, bizType, oldFiles);
    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => File.AddFilesAsync(db, files, bizId, bizType);
    #endregion

    #region Flow
    public Task CreateFlowAsync(Database db, FlowBizInfo info) => Flow.CreateFlowAsync(db, info);
    public Task DeleteFlowAsync(Database db, string bizId) => Flow.DeleteFlowAsync(db, bizId);
    public Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note) => Flow.AddFlowLogAsync(db, bizId, stepName, result, note);
    public Task<List<SysFlowLog>> GetFlowLogsAsync(string bizId) => Flow.GetFlowLogsAsync(bizId);
    #endregion
}