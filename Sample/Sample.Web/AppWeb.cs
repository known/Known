namespace Sample.Web;

class AppWeb
{
    internal static void Run(WebApplicationBuilder builder)
    {
        Initialize(builder);

        builder.RunAsBlazorServer(services =>
        {
            services.AddApp();
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000") });
        });
    }

    private static void Initialize(WebApplicationBuilder builder)
    {
        Config.IsWebApi = false;
        KCConfig.WebRoot = builder.Environment.WebRootPath;
        KCConfig.ContentRoot = builder.Environment.ContentRootPath;

        var configuration = builder.Configuration;
        var dbFile = configuration.GetSection("DBFile").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();
        Initialize(dbFile, uploadPath);
    }

    private static void Initialize(string? dbFile, string? uploadPath)
    {
        var rootPath = KCConfig.ContentRoot;
        if (!string.IsNullOrWhiteSpace(dbFile))
            dbFile = Path.GetFullPath(Path.Combine(rootPath, dbFile));
        if (!string.IsNullOrWhiteSpace(uploadPath))
            uploadPath = Path.GetFullPath(Path.Combine(rootPath, uploadPath));

        AppConfig.Initialize();
        AppRazor.Initialize();
        AppCore.Initialize();
        //初始化资源文件默认数据库
        AppCore.InitDatabase(dbFile);
        AppCore.RegisterServices();

        //注册数据库访问提供者
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
            //["Npgsql"] = typeof(Npgsql.NpgsqlFactory)
            //["MySql"] = typeof(MySqlConnector.MySqlConnectorFactory)
            //["Access"] = typeof(System.Data.OleDb.OleDbFactory)
            //["SqlClient"] = typeof(System.Data.SqlClient.SqlClientFactory)
        });
        //配置数据库连接
        var connInfo = new Known.Core.ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = $"Data Source={dbFile};"
        };
        KCConfig.App = new AppInfo
        {
            Connections = new List<Known.Core.ConnectionInfo> { connInfo },
            UploadPath = uploadPath
        };
    }
}