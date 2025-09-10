namespace Known.Services;

partial class PlatformService
{
    public async Task<List<PluginInfo>> GetTopNavsAsync()
    {
        var datas = await Database.GetConfigAsync<List<PluginInfo>>(Constant.KeyTopNav, true);
        datas ??= [];
        return datas;
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Database.SaveConfigAsync(Constant.KeyTopNav, infos, true);
    }
}