﻿namespace Known.Services;

public interface ISystemService : IService
{
    Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria);
    Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria);
    [AllowAnonymous] Task<InstallInfo> GetInstallAsync();
    [AllowAnonymous] Task<SystemInfo> GetSystemAsync();
    Task<SystemDataInfo> GetSystemDataAsync();
    [AllowAnonymous] Task<Result> TestConnectionAsync(DatabaseInfo info);
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);
    Task<Result> SaveSystemAsync(SystemInfo info);
    Task<Result> SaveKeyAsync(SystemInfo info);
    Task<Result> AddLogAsync(SysLog log);
}

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    internal const string KeySystem = "SystemInfo";

    //Config
    public static async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await SystemRepository.GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }

    public static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        await SystemRepository.SaveConfigAsync(db, key, json);
    }

    //Install
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = await GetInstallDataAysnc();
        info.Databases = Config.App.Connections.Select(c => new DatabaseInfo
        {
            Name = c.Name,
            Type = c.DatabaseType.ToString(),
            ConnectionString = c.GetDefaultConnectionString()
        }).ToList();
        return info;
    }

    public async Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        try
        {
            var type = Utils.ConvertTo<DatabaseType>(info.Type);
            var database = new Database(type, info.ConnectionString);
            await database.OpenAsync();
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

        Config.App.SetConnection(info.Databases);
        await Database.InitializeAsync();

        var modules = ModuleHelper.GetModules();
        var company = GetCompany(info);
        var user = GetUser(info);
        var orga = GetOrganization(info);
        var sys = GetSystem(info);
        var database = GetDatabase(user);
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            await SaveConfigAsync(db, KeySystem, sys);
            await db.SaveDatasAsync(modules);
            await db.SaveAsync(company);
            await db.SaveAsync(user);
            await db.SaveAsync(orga);
        });
        if (result.IsValid)
        {
            AppHelper.SaveProductKey(info.ProductKey);
            result.Data = await GetInstallDataAysnc();
        }
        return result;
    }

    private Database GetDatabase(SysUser user)
    {
        return new Database
        {
            Context = Context,
            User = new UserInfo
            {
                AppId = user.AppId,
                CompNo = user.CompNo,
                UserName = user.UserName,
                Name = user.Name
            }
        };
    }

    //System
    public async Task<SystemInfo> GetSystemAsync()
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
        try
        {
            if (!Config.App.IsPlatform || db.User == null)
            {
                var json = await SystemRepository.GetConfigAsync(db, KeySystem);
                return Utils.FromJson<SystemInfo>(json);
            }

            var company = await CompanyRepository.GetCompanyAsync(db);
            if (company == null)
                return GetSystem(db.User);

            return Utils.FromJson<SystemInfo>(company.SystemData);
        }
        catch
        {
            //系统未安装，返回null
            return null;
        }
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        AppHelper.SaveProductKey(info.ProductKey);
        await SaveConfigAsync(Database, KeySystem, info);
        return await CheckKeyAsync();
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        if (Config.App.IsPlatform)
        {
            var company = await CompanyRepository.GetCompanyAsync(Database);
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

    private static SysCompany GetCompany(InstallInfo info)
    {
        var sys = GetSystem(info);
        return new SysCompany
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            Code = info.CompNo,
            Name = info.CompName,
            SystemData = Utils.ToJson(sys)
        };
    }

    private static SysUser GetUser(InstallInfo info)
    {
        return new SysUser
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            OrgNo = info.CompNo,
            UserName = info.AdminName.ToLower(),
            Password = Utils.ToMd5(info.AdminPassword),
            Name = info.AdminName,
            EnglishName = info.AdminName,
            Gender = "Male",
            Role = "Admin",
            Enabled = true
        };
    }

    private static SysOrganization GetOrganization(InstallInfo info)
    {
        return new SysOrganization
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            ParentId = "0",
            Code = info.CompNo,
            Name = info.CompName
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
        return SystemRepository.QueryTasksAsync(Database, criteria);
    }

    //Log
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysLog.CreateTime)} desc"];
        return SystemRepository.QueryLogsAsync(Database, criteria);
    }

    //internal async Task<Result> DeleteLogsAsync(string data)
    //{
    //    var ids = Utils.FromJson<string[]>(data);
    //    var entities = await Database.QueryListByIdAsync<SysLog>(ids);
    //    if (entities == null || entities.Count == 0)
    //        return Result.Error(Language.SelectOneAtLeast);

    //    return await Database.TransactionAsync(Language.Delete, async db =>
    //    {
    //        foreach (var item in entities)
    //        {
    //            await db.DeleteAsync(item);
    //        }
    //    });
    //}

    public async Task<Result> AddLogAsync(SysLog log)
    {
        if (log.Type == LogType.Page.ToString() &&
            string.IsNullOrWhiteSpace(log.Target) &&
            !string.IsNullOrWhiteSpace(log.Content))
        {
            var module = log.Content.StartsWith("/page/")
                       ? await Database.QueryByIdAsync<SysModule>(log.Content.Substring(6))
                       : await ModuleRepository.GetModuleByUrlAsync(Database, log.Content);
            log.Target = module?.Name;
        }

        await Database.SaveAsync(log);
        return Result.Success(Language.Success(Language.Save));
    }
}