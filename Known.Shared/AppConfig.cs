﻿using Coravel;
using Coravel.Invocable;
using Known.AntBlazor;
using Known.BootBlazor;
using Known.Cells;
using Known.Demo;
using Known.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Shared;

public enum UIType { AntDesign, Bootstrap }

public static class AppConfig
{
    public const string Branch = "Known";
    public const string SubTitle = "基于Blazor的企业级快速开发框架";

    public static void AddApp(this IServiceCollection services, Action<AppInfo> action = null)
    {
        //1.添加Known框架
        services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = "KIMS";
            info.Name = "Known信息管理系统";
            info.Type = AppType.Web;
            info.Assembly = typeof(AppConfig).Assembly;
            info.IsLanguage = true;
            info.IsTheme = true;
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            //数据库连接
            info.Connections = [new ConnectionInfo
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
        services.AddKnownAntDesign();
        //添加KnownBootstrap
        services.AddKnownBootstrap();

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

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}