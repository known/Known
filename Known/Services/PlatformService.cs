namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public interface IPlatformService : IService
{
    /// <summary>
    /// 异步保存菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveMenuAsync(MenuInfo info);
}

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return MenuHelper.SaveMenuAsync(info);
    }
}

class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/SaveMenu", info);
    }
}