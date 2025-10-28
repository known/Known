namespace Known.Services;

/// <summary>
/// 配置服务接口。
/// </summary>
public interface IConfigService : IService
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

[Client]
class ConfigClient(HttpClient http) : ClientBase(http), IConfigService
{
    public Task<List<PluginInfo>> GetTopNavsAsync() => Http.GetAsync<List<PluginInfo>>("/Config/GetTopNavs");
    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos) => Http.PostAsync("/Config/SaveTopNavs", infos);
}

[WebApi, Service]
partial class ConfigService(Context context) : ServiceBase(context), IConfigService
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