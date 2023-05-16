namespace Template.Server;

class AppServer
{
    internal static void Initialize(WebApplicationBuilder builder)
    {
        KCConfig.WebRoot = builder.Environment.WebRootPath;
        KCConfig.ContentRoot = builder.Environment.ContentRootPath;

        var configuration = builder.Configuration;
        var dbFile = configuration.GetSection("DBFile").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();
        Initialize(dbFile, uploadPath);
    }

    internal static void Initialize(string? dbFile, string? uploadPath)
    {
        AppConfig.Initialize();
        AppCore.Initialize();
		
        var path = KCConfig.ContentRoot;
        dbFile = Path.GetFullPath(Path.Combine(path, dbFile));
        uploadPath = Path.GetFullPath(Path.Combine(path, uploadPath));

        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
        });
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