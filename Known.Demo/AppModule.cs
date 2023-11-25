using Coravel;
using Known.AntBlazor;
using Known.Cells;
using Known.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Demo;

public static class AppModule
{
    public static void AddDemo(this IServiceCollection services, Action<AppInfo> action = null)
    {
        //1.添加Known框架
        services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = "KIMS";
            info.Name = "Known信息管理系统";
            info.Type = AppType.Web;
            info.Assembly = typeof(AppModule).Assembly;
            //数据库连接
            info.Connections = [new Known.ConnectionInfo
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

        //3.添加KnownAntDesign页面
        services.AddKnownAntDesign(option =>
        {
            //添加页脚内容
            var html = $@"
<span>{Config.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
<a href=""http://known.pumantech.com"" target=""_blank"">Known</a>
";
            option.Footer = b => b.Markup(html);
        });

        //4.添加定时任务
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();

        //5.添加Demo
        //添加数据字典类别
        Cache.AddDicCategory<AppDictionary>();

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }

    public static void UseDemo(this WebApplication app)
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