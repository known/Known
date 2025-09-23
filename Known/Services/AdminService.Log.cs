namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="info">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo info);
}

partial class AdminClient
{
    public Task<Result> AddLogAsync(LogInfo info) => Http.PostAsync("/Admin/AddLog", info);
}

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Database.AddLogAsync(info);
    }
}