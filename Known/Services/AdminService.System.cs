namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns>系统信息。</returns>
    Task<SystemInfo> GetSystemAsync();

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveSystemAsync(SystemInfo info);

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

partial class AdminClient
{
    public Task<SystemInfo> GetSystemAsync() => Http.GetAsync<SystemInfo>("/Admin/GetSystem");
    public Task<Result> SaveSystemAsync(SystemInfo info) => Http.PostAsync("/Admin/SaveSystem", info);
    public Task<SystemInfo> GetProductAsync() => Http.GetAsync<SystemInfo>("/Admin/GetProduct");
    public Task<Result> SaveProductKeyAsync(ActiveInfo info) => Http.PostAsync("/Admin/SaveProductKey", info);
}

partial class AdminService
{
    public Task<SystemInfo> GetSystemAsync()
    {
        return Database.GetSystemAsync();
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        await Database.SaveSystemAsync(info);
        CoreConfig.System = info;
        return Result.Success(Language.SaveSuccess);
    }

    public async Task<SystemInfo> GetProductAsync()
    {
        var info = await Database.GetSystemAsync();
        if (info != null)
        {
            info.ProductId = CoreConfig.ProductId;
            info.UserDefaultPwd = null;
        }
        return info;
    }

    public async Task<Result> SaveProductKeyAsync(ActiveInfo info)
    {
        var db = Database;
        if (info.Type == ActiveType.System)
        {
            var sys = await db.GetSystemAsync();
            sys.ProductId = info.ProductId;
            sys.ProductKey = info.ProductKey;
            await db.SaveSystemAsync(sys);
            CoreConfig.System = sys;
            return CoreConfig.CheckSystemInfo(sys);
        }
        else if (info.Type == ActiveType.Version)
        {
            if (CoreConfig.OnActiveSystem != null)
            {
                return await CoreConfig.OnActiveSystem.Invoke(db, info);
            }
        }

        foreach (var item in CoreConfig.Actives)
        {
            var result = item.Invoke(info);
            if (!result.IsValid)
                return result;
        }
        return Result.Success("");
    }
}