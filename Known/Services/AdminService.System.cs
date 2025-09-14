namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns>系统信息。</returns>
    [AllowAnonymous] Task<SystemInfo> GetSystemAsync();

    /// <summary>
    /// 异步获取产品信息。
    /// </summary>
    /// <returns>产品信息。</returns>
    Task<SystemInfo> GetProductAsync();

    /// <summary>
    /// 异步保存产品Key。
    /// </summary>
    /// <param name="info">系统激活信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveProductKeyAsync(ActiveInfo info);
}

partial class AdminService
{
    public Task<SystemInfo> GetSystemAsync()
    {
        return Task.FromResult(GetSystem());
    }

    public Task<SystemInfo> GetProductAsync()
    {
        return Task.FromResult(GetSystem());
    }

    public Task<Result> SaveProductKeyAsync(ActiveInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    private static SystemInfo GetSystem()
    {
        return Config.System ?? new SystemInfo
        {
            CompNo = Constants.CompNo,
            CompName = Constants.CompName,
            AppName = Config.App.Name
        };
    }
}

partial class AdminClient
{
    public Task<SystemInfo> GetSystemAsync()
    {
        return Http.GetAsync<SystemInfo>("/Admin/GetSystem");
    }

    public Task<SystemInfo> GetProductAsync()
    {
        return Http.GetAsync<SystemInfo>("/Admin/GetProduct");
    }

    public Task<Result> SaveProductKeyAsync(ActiveInfo info)
    {
        return Http.PostAsync("/Admin/SaveProductKey", info);
    }
}