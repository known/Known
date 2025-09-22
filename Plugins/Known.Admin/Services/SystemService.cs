namespace Known.Services;

public interface ISystemService : IService
{
    Task<SystemDataInfo> GetSystemDataAsync();
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
        CoreConfig.System = info;
        return Result.Success(Language.SaveSuccess);
    }
}