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
partial class ConfigService(Context context) : SysServiceBase(context), IConfigService
{
    public async Task<List<PluginInfo>> GetTopNavsAsync()
    {
        var datas = await Database.GetConfigAsync<List<PluginInfo>>(Constants.KeyTopNav, true);
        if (datas == null || datas.Count == 0)
            return [];

        var items = new List<PluginInfo>();
        foreach (var item in datas)
        {
            if (item.Type == typeof(NavFontSize).FullName && !Config.App.IsSize)
                continue;
            if (item.Type == typeof(NavLanguage).FullName && !Config.App.IsLanguage)
                continue;
            if (item.Type == typeof(NavTheme).FullName && !Config.App.IsTheme)
                continue;
            if (item.Type == typeof(NavUser).FullName && Config.App.Layout == LayoutType.Side)
                continue;

            items.Add(item);
        }
        return items;
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Database.SaveConfigAsync(Constants.KeyTopNav, infos, true);
    }
}