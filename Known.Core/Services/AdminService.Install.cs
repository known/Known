﻿namespace Known.Services;

partial class AdminService
{
    [AllowAnonymous]
    public async Task<InitialInfo> GetInitialAsync()
    {
        var database = Database;
        if (Language.Settings == null || Language.Settings.Count == 0)
            await AppHelper.LoadLanguagesAsync(database);

        Config.System ??= await database.GetSystemAsync();
        var info = new InitialInfo
        {
            IsInstalled = Config.System != null,
            System = Config.System,
            LanguageSettings = Language.Settings,
            Languages = Language.Datas
        };
        if (info.System != null)
        {
            info.System.ProductId = "";
            info.System.ProductKey = "";
            info.System.UserDefaultPwd = "";
        }
        CoreConfig.Load(info);
        if (CoreConfig.OnInitial != null)
            await CoreConfig.OnInitial.Invoke(database, info);
        return info;
    }

    [AllowAnonymous]
    public async Task<InstallInfo> GetInstallAsync()
    {
        if (Config.System != null)
            return new InstallInfo();

        var info = await GetInstallDataAysnc(false);
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

    [AllowAnonymous]
    public async Task<Result> TestConnectionAsync(Data.ConnectionInfo info)
    {
        if (Config.System != null)
            return Result.Error("The system is installed.");

        try
        {
            var db = Database.Create(info.Name);
            await db.OpenAsync();
            return Result.Success(CoreLanguage.ConnectSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    [AllowAnonymous]
    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (Config.System != null)
            return Result.Error("The system is installed.");

        if (info == null)
            return Result.Error(CoreLanguage.TipInstallRequired);

        if (info.AdminPassword != info.Password1)
            return Result.Error(CoreLanguage.TipPwdNotEqual);

        Console.WriteLine("Known Install");
        Console.WriteLine($"{info.CompNo}-{info.CompName}");
        AppHelper.SetConnections(info);
        var database = GetDatabase(info);
        await database.InitializeTableAsync();
        await database.MigrateDataAsync();
        var result = await database.TransactionAsync(Language.Install, async db =>
        {
            if (Config.OnInstallModules != null)
            {
                Console.WriteLine("Module is installing...");
                await Config.OnInstallModules.Invoke(db);
                Console.WriteLine("Module is installed.");
            }
            var sys = CreateSystemInfo(info);
            await db.SaveSystemAsync(sys);
            await SaveUserAsync(db, info);
            await SaveOrganizationAsync(db, info);
            await SaveCompanyAsync(db, info, sys);
        });
        if (result.IsValid)
            result.Data = await GetInstallDataAysnc(true);
        return result;
    }

    private static SystemInfo CreateSystemInfo(InstallInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = CoreOption.Instance.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };
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
        CoreOption.Instance.OnNewUser?.Invoke(db, user);
        await db.SaveAsync(user);
    }

    private static async Task SaveOrganizationAsync(Database db, InstallInfo info)
    {
        var org = await db.QueryAsync<SysOrganization>(d => d.CompNo == info.CompNo && d.Code == info.CompNo);
        org ??= new SysOrganization();
        org.AppId = Config.App.Id;
        org.CompNo = info.CompNo;
        org.ParentId = "0";
        org.Code = info.CompNo;
        org.Name = info.CompName;
        await db.SaveAsync(org);
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
                ProductId = CoreOption.Instance.ProductId,
                ProductKey = sys?.ProductKey,
                AdminName = Constants.SysUserName
            };
            if (isCheck)
                CoreOption.Instance.CheckSystemInfo(sys);
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
}