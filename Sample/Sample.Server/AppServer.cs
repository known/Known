namespace Sample.Server;

class AppServer
{
    internal static void Run(WebApplicationBuilder builder)
    {
        Initialize(builder);

        builder.RunAsBlazorWebAssembly(services =>
        {
            services.AddApp();
        }, app =>
        {
            if (KCConfig.IsDevelopment)
                app.UseWebAssemblyDebugging();
            app.UseBlazorFrameworkFiles();
            app.Services.UseApp();
        });
    }

    private static void Initialize(WebApplicationBuilder builder)
    {
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
        AppCore.Initialize();
        //初始化资源文件默认数据库
        AppCore.InitDatabase(dbFile);

        //注册数据库访问提供者
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
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