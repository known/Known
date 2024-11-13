namespace Known.Services;

/// <summary>
/// 系统服务接口。
/// </summary>
public interface ISystemService : IService
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns>系统信息。</returns>
    [AllowAnonymous] Task<SystemInfo> GetSystemAsync();

    /// <summary>
    /// 异步获取系统安装信息。
    /// </summary>
    /// <returns>系统安装信息。</returns>
    [AllowAnonymous] Task<InstallInfo> GetInstallAsync();

    /// <summary>
    /// 异步测试数据库连接。
    /// </summary>
    /// <param name="info">数据库连接信息。</param>
    /// <returns>测试结果。</returns>
    [AllowAnonymous] Task<Result> TestConnectionAsync(DatabaseInfo info);

    /// <summary>
    /// 异步保存系统安装信息。
    /// </summary>
    /// <param name="info">系统安装信息。</param>
    /// <returns>保存结果。</returns>
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);

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
    Task SaveConfigAsync(ConfigInfo info);

    /// <summary>
    /// 异步获取系统数据信息。
    /// </summary>
    /// <returns>系统数据信息。</returns>
    Task<SystemDataInfo> GetSystemDataAsync();

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveSystemAsync(SystemInfo info);

    /// <summary>
    /// 异步保存产品Key。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveKeyAsync(SystemInfo info);
}

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    public Task<SystemInfo> GetSystemAsync()
    {
        var info = new SystemInfo { AppName = App.Name };
        return Task.FromResult(info);
    }

    public Task<InstallInfo> GetInstallAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveInstallAsync(InstallInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetConfigAsync(string key)
    {
        return Task.FromResult(string.Empty);
    }

    public Task SaveConfigAsync(ConfigInfo info)
    {
        return Task.CompletedTask;
    }

    public Task<SystemDataInfo> GetSystemDataAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveSystemAsync(SystemInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveKeyAsync(SystemInfo info)
    {
        throw new NotImplementedException();
    }
}