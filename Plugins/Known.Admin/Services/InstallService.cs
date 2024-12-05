namespace Known.Services;

/// <summary>
/// 系统安装服务接口。
/// </summary>
public interface IInstallService : IService
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
    [AllowAnonymous] Task<Result> TestConnectionAsync(DatabaseInfo info);

    /// <summary>
    /// 异步保存系统安装信息。
    /// </summary>
    /// <param name="info">系统安装信息。</param>
    /// <returns>保存结果。</returns>
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);
}

class InstallService(Context context) : ServiceBase(context), IInstallService
{
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = await GetInstallDataAysnc(false);
        info.Databases = DbConfig.GetDatabases();
        info.IsDatabase = info.Databases?.Count(d => string.IsNullOrWhiteSpace(d.ConnectionString)) > 0;
        if (info.IsDatabase)
        {
            foreach (var item in info.Databases)
            {
                var type = Utils.ConvertTo<DatabaseType>(item.Type);
                item.ConnectionString = GetDefaultConnectionString(type);
            }
        }
        return info;
    }

    public async Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        try
        {
            var db = Database.Create(info.Name);
            await db.OpenAsync();
            return Result.Success(Language["Tip.ConnectSuccess"]);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.InstallRequired"]);

        if (info.AdminPassword != info.Password1)
            return Result.Error(Language["Tip.PwdNotEqual"]);

        Console.WriteLine("Known Install");
        Console.WriteLine($"{info.CompNo}-{info.CompName}");
        AppHelper.SetConnections(info);
        var database = GetDatabase(info);
        await database.InitializeTableAsync();
        Console.WriteLine("Module is installing...");
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            // 保存模块
            var modules = ModuleHelper.GetModules();
            await db.DeleteAllAsync<SysModule>();
            await db.InsertAsync(modules);

            // 保存配置
            var sys = new SystemInfo
            {
                CompNo = info.CompNo,
                CompName = info.CompName,
                AppName = info.AppName,
                ProductId = info.ProductId,
                ProductKey = info.ProductKey,
                UserDefaultPwd = "888888"
            };
            await db.SaveSystemAsync(sys);

            // 保存用户
            var userName = info.AdminName.ToLower();
            var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
            user ??= new SysUser();
            user.AppId = Config.App.Id;
            user.CompNo = info.CompNo;
            user.OrgNo = info.CompNo;
            user.UserName = userName;
            user.Password = Utils.ToMd5(info.AdminPassword);
            user.Name = info.AdminName;
            user.EnglishName = info.AdminName;
            user.Gender = "Male";
            user.Role = "Admin";
            user.Enabled = true;
            await db.SaveAsync(user);

            // 保存组织架构
            var org = await db.QueryAsync<SysOrganization>(d => d.CompNo == info.CompNo && d.Code == info.CompNo);
            org ??= new SysOrganization();
            org.AppId = Config.App.Id;
            org.CompNo = info.CompNo;
            org.ParentId = "0";
            org.Code = info.CompNo;
            org.Name = info.CompName;
            await db.SaveAsync(org);

            // 保存租户
            var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
            company ??= new SysCompany();
            company.AppId = Config.App.Id;
            company.CompNo = info.CompNo;
            company.Code = info.CompNo;
            company.Name = info.CompName;
            company.SystemData = Utils.ToJson(sys);
            await db.SaveAsync(company);
        });
        if (result.IsValid)
            result.Data = await GetInstallDataAysnc(true);
        Console.WriteLine("Module is installed.");
        return result;
    }

    private async Task<InstallInfo> GetInstallDataAysnc(bool isCheck)
    {
        try
        {
            var database = Database;
            database.EnableLog = false;
            var sys = await database.GetSystemAsync();
            var info = new InstallInfo
            {
                IsInstalled = sys != null,
                AppName = Config.App.Name,
                ProductId = sys?.ProductId,
                ProductKey = sys?.ProductKey,
                AdminName = Constants.SysUserName
            };
            if (isCheck)
                AdminOption.Instance.CheckSystemInfo(sys);
            return info;
        }
        catch
        {
            return null;
        }
    }

    private Database GetDatabase(InstallInfo info)
    {
        var db = Database.Create();
        db.Context = Context;
        db.User = new UserInfo
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            UserName = info.AdminName.ToLower(),
            Name = info.AdminName
        };
        return db;
    }

    private static string GetDefaultConnectionString(DatabaseType type)
    {
        switch (type)
        {
            case DatabaseType.Access:
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password=xxx";
            case DatabaseType.SQLite:
                return "Data Source=..\\Sample.db";
            case DatabaseType.SqlServer:
                return "Data Source=localhost;Initial Catalog=Sample;User Id=xxx;Password=xxx;";
            case DatabaseType.Oracle:
                return "Data Source=localhost:1521/orcl;User Id=xxx;Password=xxx;";
            case DatabaseType.MySql:
                return "Data Source=localhost;port=3306;Initial Catalog=Sample;user id=xxx;password=xxx;Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
            case DatabaseType.PgSql:
                return "Host=localhost;Port=5432;Database=Sample;Username=xxx;Password=xxx;";
            case DatabaseType.DM:
                return "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;";
            default:
                return string.Empty;
        }
    }
}