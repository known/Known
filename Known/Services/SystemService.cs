namespace Known.Services;

public interface ISystemService : IService
{
    [AllowAnonymous] Task<SystemInfo> GetSystemAsync();
    [AllowAnonymous] Task<InstallInfo> GetInstallAsync();
    [AllowAnonymous] Task<Result> TestConnectionAsync(DatabaseInfo info);
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);

    Task<SystemDataInfo> GetSystemDataAsync();
    Task<Result> SaveSystemAsync(SystemInfo info);
    Task<Result> SaveKeyAsync(SystemInfo info);

    Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria);
    Task<Result> DeleteTasksAsync(List<SysTask> models);
    Task<Result> ResetTasksAsync(List<SysTask> models);

    Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria);
    Task<Result> AddLogAsync(SysLog log);
}

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    private const string KeySystem = "SystemInfo";

    //Config
    internal static async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }

    internal static async Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    internal static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var appId = Config.App.Id;
        var data = new Dictionary<string, object>();
        data[nameof(SysConfig.AppId)] = appId;
        data[nameof(SysConfig.ConfigKey)] = key;
        data[nameof(SysConfig.ConfigValue)] = Utils.ToJson(value);
        var scalar = await db.CountAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        if (scalar > 0)
            await db.UpdateAsync(nameof(SysConfig), "AppId,ConfigKey", data);
        else
            await db.InsertAsync(nameof(SysConfig), data);
    }

    //System
    public async Task<SystemInfo> GetSystemAsync()
    {
        try
        {
            var info = await GetSystemAsync(Database);
            if (info != null)
            {
                info.ProductKey = null;
                info.UserDefaultPwd = null;
                //var install = GetInstall();
                //info.ProductId = install.ProductId;
                //info.ProductKey = install.ProductKey;
            }
            return info;
        }
        catch
        {
            return null;//系统未安装，返回null
        }
    }

    //Install
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = await GetInstallDataAysnc();
        if (Config.App.Connections != null)
        {
            info.Databases = Config.App.Connections.Select(c => new DatabaseInfo
            {
                Name = c.Name,
                Type = c.DatabaseType.ToString(),
                ConnectionString = c.GetDefaultConnectionString()
            }).ToList();
        }
        else
        {
            var db = Database.Create();
            info.Databases = [new DatabaseInfo {
                Name = "Default", Type = db.DatabaseType.ToString(),
                ConnectionString = db.ConnectionString
            }];
        }
        return info;
    }

    public async Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        try
        {
            Config.App.SetConnection([info]);
            var db = Database.Create();
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
        Config.App.SetConnection(info.Databases);
        await DBUtils.InitializeAsync();
        Console.WriteLine("Module is installing...");
        var modules = ModuleHelper.GetModules();
        var sys = GetSystem(info);
        var database = GetDatabase(info);
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            await db.DeleteAllAsync<SysModule>();
            await db.SaveDatasAsync(modules);
            await SaveConfigAsync(db, KeySystem, sys);
            await SaveCompanyAsync(db, info, sys);
            await SaveOrganizationAsync(db, info);
            await SaveUserAsync(db, info);
        });
        if (result.IsValid)
        {
            AppHelper.SaveProductKey(info.ProductKey);
            result.Data = await GetInstallDataAysnc();
        }
        Console.WriteLine("Module is installed.");
        return result;
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

    private static async Task SaveCompanyAsync(Database db, InstallInfo info, SystemInfo sys)
    {
        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        company ??= new SysCompany();
        company.AppId = Config.App.Id;
        company.CompNo = info.CompNo;
        company.Code = info.CompNo;
        company.Name = info.CompName;
        company.SystemData = Utils.ToJson(sys);
        await db.SaveAsync(company);
    }

    private static async Task SaveOrganizationAsync(Database db, InstallInfo info)
    {
        var org = await db.QueryAsync<SysOrganization>(d => d.CompNo == db.User.CompNo && d.Code == info.CompNo);
        org ??= new SysOrganization();
        org.AppId = Config.App.Id;
        org.CompNo = info.CompNo;
        org.ParentId = "0";
        org.Code = info.CompNo;
        org.Name = info.CompName;
        await db.SaveAsync(org);
    }

    private static async Task SaveUserAsync(Database db, InstallInfo info)
    {
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
    }

    private async Task<InstallInfo> GetInstallDataAysnc()
    {
        var app = Config.App;
        var info = new InstallInfo
        {
            AppName = app.Name,
            ProductId = app.ProductId,
            ProductKey = AppHelper.GetProductKey(),
            AdminName = Constants.SysUserName
        };
        await CheckKeyAsync();
        return info;
    }

    //System
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await GetSystemAsync(Database);
        return new SystemDataInfo
        {
            System = info,
            Version = Config.Version,
            RunTime = Utils.Round((DateTime.Now - Config.StartTime).TotalHours, 2)
        };
    }

    internal static async Task<SystemInfo> GetSystemAsync(Database db)
    {
        if (!Config.App.IsPlatform || db.User == null)
        {
            var json = await GetConfigAsync(db, KeySystem);
            return Utils.FromJson<SystemInfo>(json);
        }

        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        if (company == null)
            return GetSystem(db.User);

        return Utils.FromJson<SystemInfo>(company.SystemData);
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        if (Config.App.IsPlatform)
        {
            var company = await Database.QueryAsync<SysCompany>(d => d.Code == CurrentUser.CompNo);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.SystemData = Utils.ToJson(info);
            await Database.SaveAsync(company);
        }
        else
        {
            await SaveConfigAsync(Database, KeySystem, info);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        AppHelper.SaveProductKey(info.ProductKey);
        await SaveConfigAsync(Database, KeySystem, info);
        return await CheckKeyAsync();
    }

    private static SystemInfo GetSystem(InstallInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = info.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };
    }

    private static SystemInfo GetSystem(UserInfo info)
    {
        return new SystemInfo
        {
            CompNo = info?.CompNo,
            CompName = info?.CompName,
            AppName = Config.App.Name
        };
    }

    private async Task<Result> CheckKeyAsync()
    {
        var info = await GetSystemAsync();
        return Config.App.CheckSystemInfo(info);
    }

    //Task
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysTask.CreateTime)} desc"];
        return Database.QueryPageAsync<SysTask>(criteria);
    }

    public async Task<Result> DeleteTasksAsync(List<SysTask> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> ResetTasksAsync(List<SysTask> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Reset, async db =>
        {
            foreach (var item in models)
            {
                item.Status = SysTaskStatus.Pending;
                await db.SaveAsync(item);
            }
        });
    }

    //Log
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysLog.CreateTime)} desc"];
        return Database.QueryPageAsync<SysLog>(criteria);
    }

    public async Task<Result> AddLogAsync(SysLog log)
    {
        if (log.Type == LogType.Page.ToString() &&
            string.IsNullOrWhiteSpace(log.Target) &&
            !string.IsNullOrWhiteSpace(log.Content))
        {
            var module = log.Content.StartsWith("/page/")
                       ? await Database.QueryByIdAsync<SysModule>(log.Content.Substring(6))
                       : await Database.QueryAsync<SysModule>(d => d.Url == log.Content);
            log.Target = module?.Name;
        }

        await Database.SaveAsync(log);
        return Result.Success(Language.Success(Language.Save));
    }
}