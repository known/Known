namespace Known.Web;

static class AppConfig
{
    internal static void InitApp(this WebApplicationBuilder builder)
    {
        //设置根目录
        Config.WebRoot = builder.Environment.WebRootPath;
        Config.ContentRoot = builder.Environment.ContentRootPath;

        //获取配置
        var configuration = builder.Configuration;
        var connString = configuration.GetSection("ConnString").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();
        //配置数据库连接
        var connInfo = new ConnectionInfo
        {
            Name = "Default",
            DatabaseType = DatabaseType.SQLite,
            ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
            ConnectionString = connString
        };
        Config.SetApp(new AppInfo
        {
            //项目ID、名称、类型、程序集
            Id = "KIMS",
            Name = "Known信息管理系统",
            Type = AppType.Web,
            Assembly = typeof(AppConfig).Assembly,
            //数据库连接
            Connections = [connInfo],
            //上传文件路径
            UploadPath = uploadPath,
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            JsPath = "/script.js"
        });

        //设置产品ID，根据硬件获取ID
        Config.Copyright.ProductId = $"{Config.App.Id}-000001";
    }
}