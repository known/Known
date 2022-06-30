/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-16     KnownChen    Remove UseBlazorServer
 * 2022-06-24     KnownChen    添加异步方法
 * ------------------------------------------------------------------------------- */

#if NET6_0
using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Web;

public class KHost
{
    public static void Run(string[] args, Action<AppOption> action = null)
    {
        var app = RunApp(args, action);
        app.Run(Config.App.AppUrl);
    }

    public static void RunAsync(string[] args, Action<AppOption> action = null)
    {
        var app = RunApp(args, action);
        app.RunAsync(Config.App.AppUrl);
    }

    private static WebApplication RunApp(string[] args, Action<AppOption> action)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var option = new AppOption();
        action?.Invoke(option);
        Database.RegisterProviders(option.DbFactories);

        var builder = WebApplication.CreateBuilder(args);
        
        option.App = builder.Configuration.GetSection("KApp").Get<AppInfo>();

        Config.Init(option.App);
        Config.WebRootPath = builder.Environment.WebRootPath;
        Config.ContentRootPath = builder.Environment.ContentRootPath;

        if (option.IsBlazor)
            builder.AddKBlazorApp(option);
        else
            builder.AddKMvcApp(option);

        option.Injection?.Invoke(builder.Services, option.App);
        Config.Init(option.App);
        var app = builder.Build();

        if (option.IsBlazor)
            app.UseKBlazorApp(option);
        else
            app.UseKMvcApp(option);

        return app;
    }
}
#endif