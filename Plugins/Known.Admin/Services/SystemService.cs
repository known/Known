namespace Known.Services;

/// <summary>
/// 系统服务接口。
/// </summary>
public interface ISystemService : IService
{
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
}

[Client]
class SystemClient(HttpClient http) : ClientBase(http), ISystemService
{
    public Task<SystemDataInfo> GetSystemDataAsync()
    {
        return Http.GetAsync<SystemDataInfo>("/System/GetSystemData");
    }

    public Task<Result> SaveSystemAsync(SystemInfo info)
    {
        return Http.PostAsync("/System/SaveSystem", info);
    }
}

[WebApi, Service]
class SystemService(Context context) : ServiceBase(context), ISystemService
{
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Database.GetSystemAsync();
        return new SystemDataInfo { System = info };
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
        Config.System = info;
        return Result.Success(Language.SaveSuccess);
    }
}