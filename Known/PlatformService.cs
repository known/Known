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

    #region Company
    //public Task<string> GetCompanyAsync(Database db) => CompanyService.GetCompanyAsync(db);
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
    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFilesAsync(db, bizId, oldFiles);
    public Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles) => FileService.SaveFileAsync(db, file, bizId, bizType, oldFiles);
    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => FileService.AddFilesAsync(db, files, bizId, bizType);
    #endregion

    #region Flow
    public async Task CreateFlowAsync(Database db, FlowBizInfo info)
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

    public async Task DeleteFlowAsync(Database db, string bizId)
    {
        await FlowRepository.DeleteFlowLogsAsync(db, bizId);
        await FlowRepository.DeleteFlowAsync(db, bizId);
    }

    public Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note) => FlowService.AddFlowLogAsync(db, bizId, stepName, result, note);
    #endregion
}