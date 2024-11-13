namespace Known.Core.Services;

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    //Config
    public Task<string> GetConfigAsync(string key) => Platform.GetConfigAsync(Database, key);
    public Task SaveConfigAsync(ConfigInfo info) => Platform.SaveConfigAsync(Database, info.Key, info.Value);

    //System
    public async Task<SystemInfo> GetSystemAsync()
    {
        try
        {
            var database = Database;
            database.EnableLog = false;
            var info = await Platform.GetSystemAsync(database);
            if (info != null)
            {
                info.ProductKey = null;
                info.UserDefaultPwd = null;
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
        var info = await GetInstallDataAysnc(false);
        if (Config.App.Connections != null)
        {
            info.Databases = Config.App.Connections.Select(c => new DatabaseInfo
            {
                Name = c.Name,
                Type = c.DatabaseType.ToString(),
                ConnectionString = GetDefaultConnectionString(c)
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

    private static string GetDefaultConnectionString(ConnectionInfo info)
    {
        switch (info.DatabaseType)
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

    public async Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        try
        {
            AppHelper.SetConnection([info]);
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
        AppHelper.SetConnection(info.Databases);
        await AppHelper.InitializeAsync();
        Console.WriteLine("Module is installing...");
        var modules = ModuleHelper.GetModules();
        var sys = GetSystem(info);
        var database = GetDatabase(info);
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            await db.DeleteAllAsync<SysModule>();
            await db.InsertAsync(modules);
            await Platform.SaveSystemAsync(db, sys);
            await SaveCompanyAsync(db, info, sys);
            await SaveOrganizationAsync(db, info);
            await SaveUserAsync(db, info);
        });
        if (result.IsValid)
        {
            AppHelper.SaveProductKey(info.ProductKey);
            result.Data = await GetInstallDataAysnc(true);
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

    private async Task<InstallInfo> GetInstallDataAysnc(bool isCheck)
    {
        var app = Config.App;
        var info = new InstallInfo
        {
            AppName = app.Name,
            ProductId = app.ProductId,
            ProductKey = AppHelper.GetProductKey(),
            AdminName = Constants.SysUserName
        };
        if (isCheck)
            await Platform.CheckKeyAsync(Database);
        return info;
    }

    //System
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Platform.GetSystemAsync(Database);
        return new SystemDataInfo
        {
            System = info,
            Version = Config.Version,
            RunTime = Utils.Round((DateTime.Now - Config.StartTime).TotalHours, 2)
        };
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var company = await database.QueryAsync<SysCompany>(d => d.Code == CurrentUser.CompNo);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.SystemData = Utils.ToJson(info);
            await database.SaveAsync(company);
        }
        else
        {
            await Platform.SaveSystemAsync(database, info);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        var database = Database;
        AppHelper.SaveProductKey(info.ProductKey);
        await Platform.SaveSystemAsync(database, info);
        return await Platform.CheckKeyAsync(database);
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
}