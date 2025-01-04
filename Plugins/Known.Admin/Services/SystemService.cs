namespace Known.Services;

/// <summary>
/// 系统服务接口。
/// </summary>
public interface ISystemService : IService
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

[WebApi]
class SystemService(Context context) : ServiceBase(context), ISystemService
{
    public async Task<SystemInfo> GetSystemAsync()
    {
        var database = Database;
        database.EnableLog = false;
        var info = await database.GetSystemAsync();
        if (info != null)
        {
            info.ProductId = AdminOption.Instance.ProductId;
            info.ProductKey = null;
            info.UserDefaultPwd = null;
        }
        return info;
    }

    public async Task<SystemInfo> GetProductAsync()
    {
        var info = await Database.GetSystemAsync();
        if (info != null)
        {
            info.ProductId = AdminOption.Instance.ProductId;
            info.UserDefaultPwd = null;
        }
        return info;
    }

    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Database.GetSystemAsync();
        return new SystemDataInfo
        {
            System = info,
            Version = Config.Version,
            RunTime = Utils.Round((DateTime.Now - Config.StartTime).TotalHours, 2)
        };
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
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<Result> SaveProductKeyAsync(SystemInfo info)
    {
        var db = Database;
        var sys = await db.GetSystemAsync();
        sys.ProductId = info.ProductId;
        sys.ProductKey = info.ProductKey;
        await db.SaveSystemAsync(sys);
        return AdminOption.Instance.CheckSystemInfo(sys);
    }
}