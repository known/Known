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
    /// 异步获取系统数据信息。
    /// </summary>
    /// <returns>系统数据信息。</returns>
    Task<SystemDataInfo> GetSystemDataAsync();

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveSystemAsync(SystemInfo info);

    /// <summary>
    /// 异步保存产品Key。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveProductKeyAsync(SystemInfo info);
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

    public Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = new SystemDataInfo { System = GetSystem() };
        return Task.FromResult(info);
    }

    public Task<Result> SaveSystemAsync(SystemInfo info)
    {
        return Result.SuccessAsync("保存成功！");
    }

    public Task<Result> SaveProductKeyAsync(SystemInfo info)
    {
        return Result.SuccessAsync("保存成功！");
    }

    private static SystemInfo GetSystem()
    {
        return Config.System ?? new SystemInfo
        {
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

    public Task<SystemDataInfo> GetSystemDataAsync()
    {
        return Http.GetAsync<SystemDataInfo>("/Admin/GetSystemData");
    }

    public Task<Result> SaveSystemAsync(SystemInfo info)
    {
        return Http.PostAsync("/Admin/SaveSystem", info);
    }

    public Task<Result> SaveProductKeyAsync(SystemInfo info)
    {
        return Http.PostAsync("/Admin/SaveProductKey", info);
    }
}