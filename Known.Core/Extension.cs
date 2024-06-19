﻿namespace Known.Core;

public static class Extension
{
    private static bool IsAddWebApi { get; set; }
    private static Dictionary<string, MethodInfo> ApiMethods { get; } = [];

    public static void AddKnownWin(this IServiceCollection services)
    {
        services.AddResponseCompression();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        services.AddAuthorizationCore();
        services.AddScoped<IAuthStateProvider, WinAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
        //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //        .AddCookie(options => options.LoginPath = new PathString("/login"));
    }

    public static void AddKnownWeb(this IServiceCollection services)
    {
        services.AddResponseCompression();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        //services.AddControllers();
        //services.AddScoped<IAuthStateProvider, PersistingStateProvider>();
        //services.AddScoped<AuthenticationStateProvider, PersistingStateProvider>();
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<IAuthStateProvider, WebAuthStateProvider>();
        //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //        .AddCookie(options => options.LoginPath = new PathString("/login"));

        //builder.Services.Configure<CookiePolicyOptions>(options =>
        //{
        //    options.CheckConsentNeeded = context => true;
        //    options.MinimumSameSitePolicy = SameSiteMode.None;
        //});
        //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //                .AddCookie(options =>
        //                {
        //                    //options.Cookie.Name = "Known_Auth";
        //                    //options.Cookie.HttpOnly = true;
        //                    //options.SlidingExpiration = true;
        //                    //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        //                    options.LoginPath = new PathString("/login");
        //                });

        //builder.Services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        //builder.Services.AddScoped<AuthenticationStateProvider, PersistingStateProvider>();
    }

    public static void AddKnownWebApi(this IServiceCollection services)
    {
        IsAddWebApi = true;
        foreach (var type in Config.ApiTypes)
        {
            //Console.WriteLine($"api/{type.Name}");
            var controler = type.Name[1..].Replace("Service", "");
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.IsPublic && method.DeclaringType?.Name == type.Name)
                {
                    var name = method.Name.Replace("Async", "");
                    var pattern = $"/{controler}/{name}";
                    ApiMethods[pattern] = method;
                }
            }
        }
    }

    public static void UseKnown(this WebApplication app)
    {
        //app.MapControllers();
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

        if (IsAddWebApi)
            app.UseKnownWebApi();
    }

    private static void UseKnownWebApi(this IEndpointRouteBuilder app)
    {
        //TODO:Map动态API
        foreach (var item in ApiMethods)
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
        var token = ctx.Request.Headers[Constants.KeyToken].ToString();
        var context = Context.Create(token);
        var target = Activator.CreateInstance(method.DeclaringType, context);
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