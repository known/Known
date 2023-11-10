namespace Known.Web;

static class AppConfig
{
    internal static void InitApp(this WebApplicationBuilder builder)
    {
        //设置Web及根目录
        Config.IsWeb = true;
        Config.WebRoot = builder.Environment.WebRootPath;
        Config.ContentRoot = builder.Environment.ContentRootPath;

        //设置项目ID、名称、版本和模块
        var assembly = typeof(AppConfig).Assembly;
        Config.AppId = "KIMS";
        Config.AppName = "Known信息管理系统";
        Config.SetAppVersion(assembly);
        Config.AddModule(assembly);

        //设置产品ID，根据硬件获取ID
        Config.ProductId = $"{Config.AppId}-000001";

        //设置项目JS路径，通过UI.InvokeAppVoidAsync调用JS方法
        Config.AppJsPath = "/script.js";

        //添加数据字典类别
        Cache.AddDicCategory<AppDictionary>();

        //获取配置
        var configuration = builder.Configuration;
        var connString = configuration.GetSection("ConnString").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();

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
            ConnectionString = connString
        };
        Config.App = new AppInfo
        {
            Connections = [connInfo],
            UploadPath = uploadPath
        };
    }
}

class AppDictionary
{
    public const string Test = "测试";
}