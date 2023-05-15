namespace Template.WinForm;

class AppAlone
{
    internal const string Host = "http://localhost:5000";

    internal static void Initialize()
    {
        InitDatabase();
        AppConfig.Initialize();
        AppRazor.Initialize();
        AppCore.Initialize();

        KRConfig.Assemblies.Add(typeof(AppAlone).Assembly);
        AppSetting.Load();
        InitConfigCore();
    }

    private static void InitDatabase()
    {
        var fileName = "Template.mdb";
        var path = Path.Combine(Application.StartupPath, fileName);
        if (!File.Exists(path))
        {
            Utils.EnsureFile(path);
            var assembly = typeof(AppAlone).Assembly;
            var names = assembly.GetManifestResourceNames();
            var name = names.FirstOrDefault(n => n.Contains(fileName));
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var fs = File.Create(path))
            {
                stream.CopyTo(fs);
            }
        }
    }

    private static void InitConfigCore()
    {
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["Access"] = typeof(System.Data.OleDb.OleDbFactory)
        });

        KCConfig.AddWebPlatform();
        KCConfig.WebRoot = Application.StartupPath;
        KCConfig.ContentRoot = Application.StartupPath;
        KCConfig.App = new AppInfo
        {
            Connections = new List<ConnectionInfo>
            {
                new ConnectionInfo
                {
                    Name = "Default",
                    ProviderName = "Access",
                    ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Template.mdb;Jet OLEDB:Database Password="
                }
            }
        };
    }
}