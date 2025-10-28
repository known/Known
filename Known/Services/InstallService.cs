namespace Known.Services;

/// <summary>
/// 安装服务接口。
/// </summary>
public interface IInstallService : IService
{
    /// <summary>
    /// 异步获取系统安装信息。
    /// </summary>
    /// <returns>系统安装信息。</returns>
    [Anonymous] Task<InstallInfo> GetInstallAsync();

    /// <summary>
    /// 异步测试数据库连接。
    /// </summary>
    /// <param name="info">数据库连接信息。</param>
    /// <returns>测试结果。</returns>
    [Anonymous] Task<Result> TestConnectionAsync(ConnectionInfo info);

    /// <summary>
    /// 异步保存系统安装信息。
    /// </summary>
    /// <param name="info">系统安装信息。</param>
    /// <returns>保存结果。</returns>
    [Anonymous] Task<Result> SaveInstallAsync(InstallInfo info);
}

[Client]
class InstallClient(HttpClient http) : ClientBase(http), IInstallService
{
    public Task<InstallInfo> GetInstallAsync() => Http.GetAsync<InstallInfo>("/Install/GetInstall");
    public Task<Result> TestConnectionAsync(ConnectionInfo info) => Http.PostAsync("/Install/TestConnection", info);
    public Task<Result> SaveInstallAsync(InstallInfo info) => Http.PostAsync("/Install/SaveInstall", info);
}

[WebApi, Service]
class InstallService(Context context) : ServiceBase(context), IInstallService
{
    [Anonymous]
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = await GetInstallDataAysnc(Database, false);
        if (CoreConfig.System != null)
            return info;

        info.Connections = DbConfig.GetConnections();
        info.IsDatabase = info.Connections?.Count(d => string.IsNullOrWhiteSpace(d.ConnectionString)) > 0;
        if (info.IsDatabase)
        {
            foreach (var item in info.Connections)
            {
                item.ConnectionString = item.GetDefaultConnectionString();
            }
        }
        else
        {
            info.Connections = [];
        }
        return info;
    }

    [Anonymous]
    public async Task<Result> TestConnectionAsync(Data.ConnectionInfo info)
    {
        if (CoreConfig.System != null)
            return Result.Error("The system is installed.");

        try
        {
            using var db = Database.Create(info.Name);
            await db.OpenAsync();
            return Result.Success(Language.ConnectSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    [Anonymous]
    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (CoreConfig.System != null)
            return Result.Error("The system is installed.");

        if (info == null)
            return Result.Error(Language.TipInstallRequired);

        if (info.AdminPassword != info.Password1)
            return Result.Error(Language.TipPwdNotEqual);

        Console.WriteLine("Known Install");
        Console.WriteLine($"{info.CompNo}-{info.CompName}");
        AppHelper.SetConnections(info);

        using var database = GetDatabase(info);
        var result = await database.InitializeTableAsync();
        if (!result.IsValid)
            return result;

        result = await database.MigrateDataAsync();
        if (!result.IsValid)
            return result;

        result = await database.TransactionAsync(Language.Install, async db =>
        {
            if (CoreConfig.OnInstallModules != null)
            {
                Console.WriteLine("Module is installing...");
                await CoreConfig.OnInstallModules.Invoke(db);
                Console.WriteLine("Module is installed.");
            }
            var sys = CreateSystemInfo(info);
            await db.SaveSystemAsync(sys);
            await db.SaveOrganizationAsync(info);
            await db.SaveCompanyAsync(info, sys);
            await db.SaveUserAsync(info);
            if (CoreConfig.OnInstall != null)
                await CoreConfig.OnInstall.Invoke(db, info, sys);
        });
        if (result.IsValid)
            result.Data = await GetInstallDataAysnc(database, true);
        return result;
    }

    private static SystemInfo CreateSystemInfo(InstallInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = CoreConfig.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };
    }

    private static async Task<InstallInfo> GetInstallDataAysnc(Database db, bool isCheck)
    {
        try
        {
            db.EnableLog = false;
            var sys = await db.GetSystemAsync(isCheck);
            var info = new InstallInfo
            {
                IsInstalled = sys != null,
                AppName = Config.App.Name,
                ProductId = CoreConfig.ProductId,
                ProductKey = sys?.ProductKey,
                AdminName = Constants.SysUserName
            };
            CoreConfig.System = sys;
            return info;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;
        }
    }

    private static Database GetDatabase(InstallInfo info)
    {
        var db = Database.Create();
        db.User = new UserInfo
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            UserName = info.AdminName.ToLower(),
            Name = info.AdminName
        };
        return db;
    }
}