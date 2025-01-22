namespace Known.Services;

public partial interface IAdminService
{
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
    [AllowAnonymous] Task<Result> TestConnectionAsync(ConnectionInfo info);

    /// <summary>
    /// 异步保存系统安装信息。
    /// </summary>
    /// <param name="info">系统安装信息。</param>
    /// <returns>保存结果。</returns>
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);
}

partial class AdminService
{
    public Task<InstallInfo> GetInstallAsync()
    {
        return Task.FromResult(new InstallInfo());
    }

    public Task<Result> TestConnectionAsync(ConnectionInfo info)
    {
        return Result.SuccessAsync("测试成功！");
    }

    public Task<Result> SaveInstallAsync(InstallInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class AdminClient
{
    public Task<InstallInfo> GetInstallAsync()
    {
        return Http.GetAsync<InstallInfo>("/Admin/GetInstall");
    }

    public Task<Result> TestConnectionAsync(ConnectionInfo info)
    {
        return Http.PostAsync("/Admin/TestConnection", info);
    }

    public Task<Result> SaveInstallAsync(InstallInfo info)
    {
        return Http.PostAsync("/Admin/SaveInstall", info);
    }
}