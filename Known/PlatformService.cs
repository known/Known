using Known.Entities;
using Known.Services;
using Known.WorkFlows;

namespace Known;

public class PlatformService
{
    public PlatformService(UserInfo user)
    {
        Module = new ModuleService { CurrentUser = user };
        System = new SystemService { CurrentUser = user };
        Setting = new SettingService { CurrentUser = user };
        Company = new CompanyService { CurrentUser = user };
        Dictionary = new DictionaryService { CurrentUser = user };
        File = new FileService { CurrentUser = user };
        Flow = new FlowService { CurrentUser = user };
        Role = new RoleService { CurrentUser = user };
        User = new UserService { CurrentUser = user };
        Auth = new AuthService { CurrentUser = user };
    }

    internal ModuleService Module { get; }
    internal SystemService System { get; }
    internal SettingService Setting { get; }
    internal CompanyService Company { get; }
    internal DictionaryService Dictionary { get; }
    internal FileService File { get; }
    internal FlowService Flow { get; }
    internal RoleService Role { get; }
    internal UserService User { get; }
    internal AuthService Auth { get; }

    #region Config
    public Task<T> GetConfigAsync<T>(Database db, string key) => System.GetConfigAsync<T>(db, key);
    public Task SaveConfigAsync(Database db, string key, object value) => System.SaveConfigAsync(db, key, value);
    #endregion

    #region Setting
    public Task<List<SysSetting>> GetSettingsAsync(string bizType) => Setting.GetSettingsAsync(bizType);
    public Task<T> GetSettingAsync<T>(string bizType) => Setting.GetSettingAsync<T>(bizType);
    public Task DeleteSettingAsync(Database db, string bizType) => Setting.DeleteSettingAsync(db, bizType);
    public Task SaveSettingAsync(Database db, string bizType, object bizData) => Setting.SaveSettingAsync(db, bizType, bizData);
    public Task<Result> SaveSettingAsync(string bizType, object bizData) => Setting.SaveSettingAsync(bizType, bizData);
    public Task<List<SysSetting>> GetUserSettingsAsync(string bizType) => Setting.GetUserSettingsAsync(bizType);
    public Task<T> GetUserSettingAsync<T>(string bizType) => Setting.GetUserSettingAsync<T>(bizType);
    public Task DeleteUserSettingAsync(Database db, string bizType) => Setting.DeleteUserSettingAsync(db, bizType);
    public Task<Result> DeleteUserSettingAsync(string bizType) => Setting.DeleteUserSettingAsync(bizType);
    #endregion

    #region Company
    public Task<T> GetCompanyAsync<T>() => Company.GetCompanyAsync<T>();
    public Task<Result> SaveCompanyAsync(object model) => Company.SaveCompanyAsync(model);
    #endregion

    #region User
    public Task<UserInfo> GetUserAsync(string userName) => Auth.GetUserAsync(userName);
    public Task<AdminInfo> GetAdminAsync() => Auth.GetAdminAsync();
    #endregion

    #region File
    public Task<List<SysFile>> GetFilesAsync(string bizId) => File.GetFilesAsync(bizId);
    public Task<Result> DeleteFileAsync(SysFile file) => File.DeleteFileAsync(file);
    public void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);
    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => File.DeleteFilesAsync(db, bizId, oldFiles);
    public Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles) => File.SaveFileAsync(db, file, bizId, bizType, oldFiles);
    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => File.AddFilesAsync(db, files, bizId, bizType);
    #endregion

    #region Flow
    public Task CreateFlowAsync(Database db, FlowBizInfo info) => Flow.CreateFlowAsync(db, info);
    public Task DeleteFlowAsync(Database db, string bizId) => Flow.DeleteFlowAsync(db, bizId);
    public Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note) => Flow.AddFlowLogAsync(db, bizId, stepName, result, note);
    #endregion
}