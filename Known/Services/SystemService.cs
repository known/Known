using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class SystemService : ServiceBase
{
    internal const string KeySystem = "SystemInfo";

    //Config
    public async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await SystemRepository.GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }

    public async Task SaveConfigAsync(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        await SystemRepository.SaveConfigAsync(db, key, json);
    }

    public async Task<T> GetConfigAsync<T>(string key)
    {
        var json = await SystemRepository.GetConfigAsync(Database, key);
        return Utils.FromJson<T>(json);
    }

    public async Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        await Platform.SaveConfigAsync(Database, info.Key, info.Value);
        return Result.Success("保存成功！");
    }

    //Install
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = GetInstall();
        var sys = await GetSystemAsync(Database);
        info.IsInstalled = sys != null;
        return info;
    }

    //public async Task<Result> UpdateKeyAsync(InstallInfo info)
    //{
    //    if (info == null)
    //        return Result.Error("安装信息不能为空！");

    //    if (!Utils.HasNetwork())
    //        return Result.Error("电脑未联网，无法获取产品密钥！");

    //    var result = await PlatformHelper.UpdateKeyAsync?.Invoke(info);
    //    return result ?? Result.Success("");
    //    return Result.Success("");
    //}

    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (info == null)
            return Result.Error("安装信息不能为空！");

        if (info.Password != info.Password1)
            return Result.Error("确认密码不一致！");

        var database = Database;
        var company = GetCompany(info);
        var user = GetUser(info);
        var orga = GetOrganization(info);
        var sys = GetSystem(info);
        database.User = new UserInfo { AppId = user.AppId, CompNo = user.CompNo, UserName = user.UserName, Name = user.Name };
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);

        var result = await database.TransactionAsync("安装", async db =>
        {
            await Platform.SaveConfigAsync(db, KeySystem, sys);
            await db.SaveAsync(company);
            await db.SaveAsync(user);
            await db.SaveAsync(orga);
        });
        if (result.IsValid)
            result.Data = await GetInstallAsync();
        return result;
    }

    //System
    public async Task<SystemInfo> GetSystemAsync()
    {
        var info = await GetSystemAsync(Database);
        if (info != null)
        {
            var install = GetInstall();
            info.ProductId = install.ProductId;
            info.ProductKey = install.ProductKey;

            var config = await Platform.GetConfigAsync<SystemInfo>(Database, KeySystem);
            if (config != null)
            {
                info.Copyright = config.Copyright;
                info.SoftTerms = config.SoftTerms;
            }
        }
        return info;
    }

    internal static async Task<SystemInfo> GetSystemAsync(Database db)
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

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);
        await Platform.SaveConfigAsync(Database, KeySystem, info);
        var result = await CheckKeyAsync();
        return result;
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        if (Config.App.IsPlatform)
        {
            var company = await CompanyRepository.GetCompanyAsync(Database);
            if (company == null)
                return Result.Error("企业不存在！");

            company.SystemData = Utils.ToJson(info);
            await Database.SaveAsync(company);
        }
        else
        {
            await Platform.SaveConfigAsync(Database, KeySystem, info);
        }

        return Result.Success("保存成功！");
    }

    public async Task<Result> SaveSystemConfigAsync(SystemInfo info)
    {
        await Platform.SaveConfigAsync(Database, KeySystem, info);
        return Result.Success("保存成功！");
    }

    private static InstallInfo GetInstall()
    {
        var app = Config.App;
        var path = GetProductKeyPath();
        var info = new InstallInfo
        {
            AppName = app.Name,
            ProductId = app.ProductId,
            ProductKey = Utils.ReadFile(path),
            UserName = Constants.SysUserName
        };
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
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = Config.App.Name,
            UserDefaultPwd = "888888"
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
            UserName = info.UserName.ToLower(),
            Password = Utils.ToMd5(info.Password),
            Name = "管理员",
            EnglishName = info.UserName,
            Gender = "男",
            Role = "管理员",
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

    private static string GetProductKeyPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", $"{Config.App.Id}.key");
    }

    private async Task<Result> CheckKeyAsync()
    {
        var info = await GetSystemAsync();
        //var result = PlatformHelper.CheckSystem?.Invoke(Database, info);
        //return result ?? Result.Success("");
        return Result.Success("");
    }

    //Task
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        return SystemRepository.QueryTasksAsync(Database, criteria);
    }

    //Log
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        return SystemRepository.QueryLogsAsync(Database, criteria);
    }

    internal async Task<Result> DeleteLogsAsync(string data)
    {
        var ids = Utils.FromJson<string[]>(data);
        var entities = await Database.QueryListByIdAsync<SysLog>(ids);
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in entities)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    internal Task<List<SysLog>> GetLogsAsync(string bizId) => SystemRepository.GetLogsAsync(Database, bizId);

    public async Task<Result> AddLogAsync(SysLog log)
    {
        await Database.SaveAsync(log);
        return Result.Success("添加成功！");
    }
}