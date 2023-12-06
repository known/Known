using Coravel;
using Known.AntBlazor;
using Known.BootBlazor;
using Known.Cells;
using Known.Demo;
using Known.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Shared;

public static class Extension
{
    public static void AddApp(this IServiceCollection services, Action<AppInfo> action = null)
    {
        //1.添加Known框架
        services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = "KIMS";
            info.Name = "Known信息管理系统";
            info.Type = AppType.Web;
            info.Assembly = typeof(Extension).Assembly;
            //数据库连接
            info.Connections = [new ConnectionInfo
            {
                Name = "Default",
                DatabaseType = DatabaseType.SQLite,
                ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
                //ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
            }];
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            info.JsPath = "./script.js";
            action?.Invoke(info);
        });

        //2.添加KnownExcel实现
        services.AddKnownCells();

        //3.添加UI扩展库
        //添加KnownAntDesign
        services.AddKnownAntDesign(option =>
        {
            //添加页脚内容
            var html = $@"
<span>{Config.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
<a href=""http://known.pumantech.com"" target=""_blank"">Known</a>
";
            option.Footer = b => b.Markup(html);
        });
        //添加KnownBootstrap
        //services.AddKnownBootstrap();

        //4.添加Demo
        services.AddDemoModule();

        //5.添加定时任务
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    public static void UseApp(this WebApplication app)
    {
        //6.配置定时任务
        app.Services.UseScheduler(scheduler =>
        {
            //每5秒执行一次异步导入
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });

        app.UseStaticFiles();
        //7.使用Known框架静态文件
        app.UseKnownStaticFiles();
    }
}