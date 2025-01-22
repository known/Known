namespace Known.Services;

public partial interface IPlatformService
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
}

partial class PlatformService
{
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Task.FromResult(AppData.Data.TopNavs);
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        AppData.Data.TopNavs = infos;
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class PlatformClient
{
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Http.GetAsync<List<PluginInfo>>("/Platform/GetTopNavs");
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Http.PostAsync("/Platform/SaveTopNavs", infos);
    }
}