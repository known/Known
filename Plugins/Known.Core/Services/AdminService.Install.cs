namespace Known.Services;

partial class AdminService
{
    [AllowAnonymous]
    public async Task<InitialInfo> GetInitialAsync()
    {
        var database = Database;
        if (Language.Settings == null || Language.Settings.Count == 0)
            await AppHelper.LoadLanguagesAsync(database);

        var sys = await database.GetSystemAsync();
        var info = new InitialInfo
        {
            IsInstalled = sys != null,
            LanguageSettings = Language.Settings,
            Languages = Language.Datas
        };
        if (sys != null)
        {
            info.System = sys.Clone();
            info.System.ProductId = null;
            info.System.ProductKey = null;
            info.System.UserDefaultPwd = null;
        }
        CoreConfig.System = sys;
        CoreConfig.Load(info);
        if (CoreConfig.OnInitial != null)
            await CoreConfig.OnInitial.Invoke(database, info);
        return info;
    }

    [AllowAnonymous]
    public async Task<InstallInfo> GetInstallAsync()
    {
        if (CoreConfig.System != null)
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
        if (CoreConfig.System != null)
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
        if (CoreConfig.System != null)
            return Result.Error("The system is installed.");

        if (info == null)
            return Result.Error(CoreLanguage.TipInstallRequired);

        if (info.AdminPassword != info.Password1)
            return Result.Error(CoreLanguage.TipPwdNotEqual);

        Console.WriteLine("Known Install");
        Console.WriteLine($"{info.CompNo}-{info.CompName}");
        AppHelper.SetConnections(info);

        var database = GetDatabase(info);
        var result = await database.InitializeTableAsync();
        if (!result.IsValid)
            return result;

        result = await database.MigrateDataAsync();
        if (!result.IsValid)
            return result;

        result = await database.TransactionAsync(Language.Install, async db =>
        {
            if (Config.OnInstallModules != null)
            {
                Console.WriteLine("Module is installing...");
                await Config.OnInstallModules.Invoke(db);
                Console.WriteLine("Module is installed.");
            }
            var sys = CreateSystemInfo(info);
            await db.SaveSystemAsync(sys);
            await db.SaveUserAsync(info);
            if (Config.OnInstall != null)
                await Config.OnInstall.Invoke(db, info, sys);
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