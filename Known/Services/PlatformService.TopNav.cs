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

partial class PlatformClient
{
    public Task<List<PluginInfo>> GetTopNavsAsync() => Http.GetAsync<List<PluginInfo>>("/Platform/GetTopNavs");
    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos) => Http.PostAsync("/Platform/SaveTopNavs", infos);
}

partial class PlatformService
{
    public async Task<List<PluginInfo>> GetTopNavsAsync()
    {
        var datas = await Database.GetConfigAsync<List<PluginInfo>>(Constants.KeyTopNav, true);
        datas ??= [];
        return datas;
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Database.SaveConfigAsync(Constants.KeyTopNav, infos, true);
    }
}