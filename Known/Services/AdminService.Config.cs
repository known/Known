namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步判断系统是否需要安装。
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous] Task<bool> GetIsInstallAsync();

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

partial class AdminService
{
    public Task<bool> GetIsInstallAsync()
    {
        return Task.FromResult(false);
    }

    public Task<string> GetConfigAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult("");

        Configs.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        if (!string.IsNullOrWhiteSpace(info.Key))
            Configs[info.Key] = Utils.ToJson(info.Value);
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class AdminClient
{
    public Task<bool> GetIsInstallAsync()
    {
        return Http.GetAsync<bool>("/Admin/GetIsInstall");
    }

    public Task<string> GetConfigAsync(string key)
    {
        return Http.GetStringAsync($"/Admin/GetConfig?key={key}");
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Http.PostAsync("/Admin/SaveConfig", info);
    }
}