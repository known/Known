namespace Sample.WinForm;

public sealed class AppAlone
{
    private const string MutexName = "Global\\KnownSample";
    internal const string Host = "http://localhost:5000";

    private AppAlone() { }

    public static void Run()
    {
        using var mutex = new Mutex(true, MutexName, out var isFirstInstance);
        if (!isFirstInstance)
            return;

        Initialize();
        Application.Run(new MainForm());
    }

    private static void Initialize()
    {
        AppConfig.Initialize(false);
        AppRazor.Initialize(false);
        AppCore.Initialize();
        
        //启动或注册后端服务
        if (Config.IsWebApi)
            AppHost.RunWebApiAsync<AppCore>(Host);
        else
            AppCore.RegisterServices();
        
        //添加WinForm程序集到前端，框架自动反射定制页面
        KRConfig.Assemblies.Add(typeof(AppAlone).Assembly);
        //加载WinForm设置
        AppSetting.Load();
        //初始化后端配置
        InitConfigCore();
    }

    private static void InitConfigCore()
    {
        KCConfig.AddWebPlatform();
        KCConfig.WebRoot = Path.Combine(Application.StartupPath, "wwwroot");
        KCConfig.ContentRoot = Application.StartupPath;

        //初始化资源文件默认数据库
        var dbFile = Path.Combine(Application.StartupPath, "Sample.db");
        AppCore.InitDatabase(dbFile);

        //注册数据库访问提供者
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
        });
        //配置数据库连接
        var connInfo = new ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = $"Data Source={dbFile};"
        };
        KCConfig.App = new AppInfo
        {
            Connections = new List<ConnectionInfo> { connInfo }
        };
    }
}