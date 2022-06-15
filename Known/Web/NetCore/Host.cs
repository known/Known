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
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var option = new AppOption();
        action?.Invoke(option);
        Database.RegisterProviders(option.DbFactories);

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpContextAccessor();

        WebAppContext.Services = builder.Services;
        Container.Register<IAppContext, WebAppContext>();

        option.App = builder.Configuration.GetSection("KApp").Get<AppInfo>();
        option.Injection?.Invoke(builder.Services, option.App);

        Config.Init(option.App);
        Config.WebRootPath = builder.Environment.WebRootPath;
        Config.ContentRootPath = builder.Environment.ContentRootPath;

        builder.AddKMvcApp(option);

        Config.Init(option.App);
        var app = builder.Build();
        app.UseKMvcApp(option);

        app.Run(Config.App.AppUrl);
    }
}
#endif