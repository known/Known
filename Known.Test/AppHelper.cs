using Known.Test.Pages;

namespace Known.Test;

class AppHelper
{
    private const string MutexName = "Global\\KnownTest";
    internal const string Url = "http://localhost:5000";

    internal static void Run()
    {
        using var mutex = new Mutex(true, MutexName, out var isFirstInstance);
        if (!isFirstInstance)
            return;

        InitDatabase();
        InitConfig();
        Application.Run(new MainForm());
    }

    private static void InitDatabase()
    {
        var fileName = "Test.db";
        var path = Path.Combine(Application.StartupPath, fileName);
        if (!File.Exists(path))
        {
            var assembly = typeof(AppHelper).Assembly;
            var names = assembly.GetManifestResourceNames();
            var name = names.FirstOrDefault(n => n.Contains(fileName));
            if (string.IsNullOrWhiteSpace(name))
                return;

            Utils.EnsureFile(path);
            using var stream = assembly.GetManifestResourceStream(name);
            using var fs = File.Create(path);
            stream?.CopyTo(fs);
        }
    }

    private static void InitConfig()
    {
        DicCategory.AddCategories<AppDictionary>();

        Config.IsPlatform = true;
        Config.IsWebApi = false;
        Config.SetAppAssembly(typeof(AppHelper).Assembly);
        if (Config.IsWebApi)
            AppHost.RunWebApiAsync<App>(Url);
        else
            KCConfig.RegisterServices();

        //KRConfig.IsWeb = true;
        KRConfig.Home = new MenuItem("首页", "fa fa-home", typeof(Home));

        KCConfig.AddWebPlatform();
        KCConfig.WebRoot = Application.StartupPath;
        KCConfig.ContentRoot = Application.StartupPath;

        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
        });
        var connInfo = new ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = $"Data Source=Test.db;"
        };
        KCConfig.App = new AppInfo
        {
            Connections = new List<ConnectionInfo> { connInfo }
        };
    }
}

public class AppDictionary
{
    public const string Test = "测试";
    public const string Type = "类型";
}