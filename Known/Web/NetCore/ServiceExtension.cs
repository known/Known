/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

#if NET6_0
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Known.Web;

public static class ServiceExtension
{
    public static WebApplicationBuilder AddKMvcApp(this WebApplicationBuilder builder, AppOption option)
    {
        var mvcBuilder = builder.Services.AddControllers(options =>
        {
            options.EnableEndpointRouting = false;
            options.Filters.Add(new ApiActionFilter());
            option.MvcOption?.Invoke(options);
        });

        mvcBuilder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            option.JsonOption?.Invoke(options);
        });

        AddDynamicApi(mvcBuilder, option);

        builder.Services.AddSession();
        builder.Services.AddRazorPages();
        return builder;
    }

    public static WebApplication UseKMvcApp(this WebApplication app, AppOption option)
    {
        if (app.Environment.IsDevelopment())
        {
            var providers = GetPhysicalFileProviders("wwwroot", "docs", "src");
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new CompositeFileProvider(providers)
            });
        }
        else
        {
            app.UseExceptionHandler("/?m=Error500");
            app.UseStaticFiles();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseWebSockets();
        app.UseMvc(routes =>
        {
            routes.Routes.Add(new WebRoute());
        });
        app.MapControllers();
        app.MapRazorPages();
        return app;
    }

    private static List<PhysicalFileProvider> GetPhysicalFileProviders(params string[] folders)
    {
        var providers = new List<PhysicalFileProvider>();
        foreach (var folder in folders)
        {
            var path = Path.Combine(Config.ContentRootPath, folder);
            if (Directory.Exists(path))
            {
                providers.Add(new PhysicalFileProvider(path));
            }
        }
        return providers;
    }

    private static void AddDynamicApi(IMvcBuilder builder, AppOption option)
    {
        builder.ConfigureApplicationPartManager(m =>
        {
            m.ApplicationParts.Add(new AssemblyPart(typeof(IService).Assembly));
            foreach (var item in option.Modules)
            {
                var module = Activator.CreateInstance(item) as IAppModule;
                module?.Initialize(option.App);
                m.ApplicationParts.Add(new AssemblyPart(item.Assembly));
            }
            m.FeatureProviders.Add(new ApiFeatureProvider());
        });

        builder.Services.Configure<MvcOptions>(o =>
        {
            o.Conventions.Add(new ApiConvention());
        });
    }
}
#endif