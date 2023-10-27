using Known.Razor;

namespace Known.Web;

class AppWeb
{
    internal static void Initialize(WebApplicationBuilder builder)
    {
        //设置根目录
        Config.WebRoot = builder.Environment.WebRootPath;
        Config.ContentRoot = builder.Environment.ContentRootPath;

        //设置项目ID和名称
        Config.AppId = "KIMS";
        Config.AppName = "Known信息管理系统";

        //设置项目Js路径
        Config.AppJsPath = "script.js";
        
        //设置默认分页大小
        PagingCriteria.DefaultPageSize = 20;

        //获取配置
        var configuration = builder.Configuration;
        var connString = configuration.GetSection("ConnString").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();

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
        var connInfo = new Known.ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = connString
        };
        Config.App = new AppInfo
        {
            Connections = new List<Known.ConnectionInfo> { connInfo },
            UploadPath = uploadPath
        };
    }
}