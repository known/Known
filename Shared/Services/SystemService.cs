namespace Known.Services;

/// <summary>
/// 系统服务接口。
/// </summary>
public interface ISystemService : IService
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns></returns>
    Task<SystemDataInfo> GetSystemDataAsync();

    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns>系统信息。</returns>
    Task<SystemInfo> GetSystemAsync();

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns></returns>
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

[Client]
class SystemClient(HttpClient http) : ClientBase(http), ISystemService
{
    public Task<SystemDataInfo> GetSystemDataAsync() => Http.GetAsync<SystemDataInfo>("/System/GetSystemData");
    public Task<SystemInfo> GetSystemAsync() => Http.GetAsync<SystemInfo>("/System/GetSystem");
    public Task<Result> SaveSystemAsync(SystemInfo info) => Http.PostAsync("/System/SaveSystem", info);
    public Task<SystemInfo> GetProductAsync() => Http.GetAsync<SystemInfo>("/System/GetProduct");
    public Task<Result> SaveProductKeyAsync(ActiveInfo info) => Http.PostAsync("/System/SaveProductKey", info);
}

[WebApi, Service]
class SystemService(Context context) : ServiceBase(context), ISystemService
{
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Database.GetSystemAsync();
        var data = new SystemDataInfo { System = info };
        data.RunTime = Utils.Round((DateTime.Now - CoreConfig.StartTime).TotalHours, 2);
        return data;
    }

    public Task<SystemInfo> GetSystemAsync()
    {
        return Database.GetSystemAsync();
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var result = await database.SaveCompanyDataAsync(CurrentUser.CompNo, info);
            if (!result.IsValid)
                return result;
        }
        else
        {
            await database.SaveSystemAsync(info);
        }
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