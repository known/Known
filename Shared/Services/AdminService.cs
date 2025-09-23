namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public partial interface IAdminService : IService
{
    /// <summary>
    /// 异步获取系统配置数据。
    /// </summary>
    /// <param name="key">配置数据键。</param>
    /// <returns>配置数据JSON字符串。</returns>
    Task<string> GetConfigAsync(string key);

    /// <summary>
    /// 异步保存系统配置数据。
    /// </summary>
    /// <param name="info">系统配置数据信息。</param>
    /// <returns></returns>
    Task<Result> SaveConfigAsync(ConfigInfo info);

    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="info">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo info);

    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="info">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo info);
}

[Client]
partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
    public Task<string> GetConfigAsync(string key) => Http.GetTextAsync($"/Admin/GetConfig?key={key}");
    public Task<Result> SaveConfigAsync(ConfigInfo info) => Http.PostAsync("/Admin/SaveConfig", info);
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria) => Http.QueryAsync<UserInfo>("/Admin/QueryUsers", criteria);
    public Task<List<AttachInfo>> GetFilesAsync(string bizId) => Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    public Task<Result> DeleteFileAsync(AttachInfo info) => Http.PostAsync("/Admin/DeleteFile", info);
    public Task<Result> AddLogAsync(LogInfo info) => Http.PostAsync("/Admin/AddLog", info);
}

[WebApi, Service]
partial class AdminService(Context context) : ServiceBase(context), IAdminService
{
    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }

    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return await Database.Query<SysUser>(criteria).ToPageAsync<UserInfo>();
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Path))
            return Result.Error(Language.TipFileNotExists);

        var oldFiles = new List<string>();
        await Database.DeleteFileAsync(info, oldFiles);
        AttachFile.DeleteFiles(oldFiles);
        return Result.Success(Language.DeleteSuccess);
    }

    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Database.AddLogAsync(info);
    }
}