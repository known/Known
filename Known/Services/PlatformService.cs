namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public interface IPlatformService : IService
{
    /// <summary>
    /// 异步获取顶部导航信息列表。
    /// </summary>
    /// <returns>顶部导航信息列表。</returns>
    Task<List<PluginInfo>> GetTopNavsAsync();

    /// <summary>
    /// 异步保存顶部导航信息列表。
    /// </summary>
    /// <param name="infos">顶部导航信息列表。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveTopNavsAsync(List<PluginInfo> infos);

    /// <summary>
    /// 异步删除菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> DeleteMenuAsync(MenuInfo info);

    /// <summary>
    /// 异步保存菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveMenuAsync(MenuInfo info);
}

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Task.FromResult(AppData.Data?.TopNavs);
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return AppData.SaveTopNavsAsync(infos);
    }

    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        return AppData.DeleteMenuAsync(info);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return AppData.SaveMenuAsync(info);
    }
}

class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Http.GetAsync<List<PluginInfo>>("/Platform/GetTopNavs");
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Http.PostAsync("/Platform/SaveTopNavs", infos);
    }

    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/DeleteMenu", info);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/SaveMenu", info);
    }
}