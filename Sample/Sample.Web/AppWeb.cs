using Coravel;
using Coravel.Invocable;
using Sample.Web.Auths;

namespace Sample.Web;

public static class AppWeb
{
    public static void AddApp(this WebApplicationBuilder builder, Action<AppInfo> action = null)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCascadingAuthenticationState();
        //builder.Services.AddScoped<IAuthStateProvider, PersistingStateProvider>();
        //builder.Services.AddScoped<AuthenticationStateProvider, PersistingStateProvider>();
        builder.Services.AddScoped<ProtectedSessionStorage>();
        builder.Services.AddScoped<IAuthStateProvider, WebAuthStateProvider>();
        //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //                .AddCookie(options => options.LoginPath = new PathString("/login"));

        //1.添加Known框架
        builder.Services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = "KIMS";
            info.Name = "Known信息管理系统";
            info.Assembly = typeof(AppWeb).Assembly;
            //info.AssemblyAdditional = true;
            info.IsLanguage = true;
            info.IsTheme = true;
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            info.JsPath = "./script.js";
            action?.Invoke(info);
        });
        builder.Services.AddKnownCore(info =>
        {
            info.WebRoot = builder.Environment.WebRootPath;
            info.ContentRoot = builder.Environment.ContentRootPath;
            //数据库连接
            info.Connections = [new Known.ConnectionInfo
            {
                Name = "Default",
                DatabaseType = DatabaseType.SQLite,
                ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
                //DatabaseType = DatabaseType.Access,
                //ProviderType = typeof(System.Data.OleDb.OleDbFactory),
                //DatabaseType = DatabaseType.MySql,
                //ProviderType = typeof(MySqlConnector.MySqlConnectorFactory),
                //DatabaseType = DatabaseType.Npgsql,
                //ProviderType = typeof(Npgsql.NpgsqlFactory),
                //DatabaseType = DatabaseType.SqlServer,
                //ProviderType = typeof(System.Data.SqlClient.SqlClientFactory),
                ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
            }];
        });
        builder.Services.AddKnownWebApi();

        //2.添加KnownExcel实现
        builder.Services.AddKnownCells();

        //3.添加UI扩展库
        //添加KnownAntDesign
        builder.Services.AddKnownAntDesign();

        //4.添加Demo
        builder.Services.AddDemo();
        Config.AddModule(typeof(Client._Imports).Assembly);

        //5.添加定时任务
        builder.Services.AddScheduler();
        builder.Services.AddTransient<ImportTaskJob>();
    }

    public static void UseApp(this WebApplication app)
    {
        //使用Known框架静态文件
        app.UseStaticFiles();
        var webFiles = Config.GetUploadPath(true);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(webFiles),
            RequestPath = "/Files"
        });
        var upload = Config.GetUploadPath();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });

        //配置认证
        //app.UseAuthentication();
        //app.UseAuthorization();

        //配置定时任务
        app.Services.UseScheduler(scheduler =>
        {
            //每5秒执行一次异步导入
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });

        //Map动态API
        foreach (var item in Config.ApiMethods)
        {
            //Console.WriteLine(item.Key);
            if (item.Value.Name.StartsWith("Get"))
                app.MapGet(item.Key, ctx => InvokeGetMethod(ctx, item.Value));
            else
                app.MapPost(item.Key, ctx => InvokePostMethod(ctx, item.Value));
        }
    }

    private static async Task InvokeGetMethod(HttpContext ctx, MethodInfo method)
    {
        var target = Activator.CreateInstance(method.DeclaringType);
        var parameters = new List<object>();
        foreach (var item in method.GetParameters())
        {
            var parameter = ctx.Request.Query[item.Name].ToString();
            parameters.Add(parameter);
        }
        var value = method.Invoke(target, [.. parameters]);
        await ctx.Response.WriteAsJsonAsync(value);
    }

    private static async Task InvokePostMethod(HttpContext ctx, MethodInfo method)
    {
        var target = Activator.CreateInstance(method.DeclaringType);
        var parameters = new List<object>();
        foreach (var item in method.GetParameters())
        {
            var parameter = ctx.Request.Form[item.Name].ToString();
            parameters.Add(parameter);
        }
        var value = method.Invoke(target, [.. parameters]);
        await ctx.Response.WriteAsJsonAsync(value);
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}