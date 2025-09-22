namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取系统配置数据。
    /// </summary>
    /// <param name="key">配置数据键。</param>
    /// <returns>配置数据JSON字符串。</returns>
    Task<string> GetConfigAsync(string key);

    /// <summary>
    /// 异步保存系统配置数据。
    /// </summary>
    /// <param name="info">系统配置数据信息。</param>
    /// <returns></returns>
    Task<Result> SaveConfigAsync(ConfigInfo info);
}

partial class AdminClient
{
    public Task<string> GetConfigAsync(string key) => Http.GetTextAsync($"/Admin/GetConfig?key={key}");
    public Task<Result> SaveConfigAsync(ConfigInfo info) => Http.PostAsync("/Admin/SaveConfig", info);
}

partial class AdminService
{
    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }
}