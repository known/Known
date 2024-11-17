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

    /// <summary>
    /// 异步保存产品Key。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveKeyAsync(SystemInfo info);
}

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Admin.GetSystemAsync(Database);
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
            await Admin.SaveSystemAsync(database, info);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        var database = Database;
        AppHelper.SaveProductKey(info.ProductKey);
        await Admin.SaveSystemAsync(database, info);
        return await Admin.CheckKeyAsync(database);
    }
}