using Known.Auths;
using Known.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Known;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class CoreExtension
{
    /// <summary>
    /// 添加桌面框架及身份认证支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置委托。</param>
    public static void AddKnownWin(this IServiceCollection services, Action<CoreOption> action = null)
    {
        AppHelper.LoadConnections();
        action?.Invoke(CoreOption.Instance);

        if (CoreOption.Instance.Database != null)
            services.AddKnownData(CoreOption.Instance.Database);
        services.AddKnownCore();
        services.AddKnownServices();

        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthStateProvider, WinAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
    }

    /// <summary>
    /// 添加Web框架及身份认证支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置委托。</param>
    public static void AddKnownWeb(this IServiceCollection services, Action<CoreOption> action = null)
    {
        AppHelper.LoadConnections();
        action?.Invoke(CoreOption.Instance);

        if (CoreOption.Instance.Database != null)
            services.AddKnownData(CoreOption.Instance.Database);
        services.AddKnownCore();
        services.AddKnownServices();

        if (CoreOption.Instance.IsCompression)
            services.AddResponseCompression();
        services.AddHttpContextAccessor();
        var builder = services.AddControllers(option =>
        {
            option.EnableEndpointRouting = false;
            option.Filters.Add<AuthActionFilter>();
            option.Filters.Add<LogActionFilter>();
            option.Filters.Add<ExceptionFilter>();
            CoreOption.Instance.Mvc?.Invoke(option);
        })
        .AddJsonOptions(option =>
        {
            //option.JsonSerializerOptions.PropertyNamingPolicy = null;
            CoreOption.Instance.Json?.Invoke(option);
        });
        if (CoreOption.Instance.IsAddWebApi)
            builder.AddDynamicWebApi();

        services.AddRazorPages();
        services.AddCascadingAuthenticationState();
        services.AddAuthProvider();
    }

    /// <summary>
    /// 使用框架静态文件和WebApi。
    /// </summary>
    /// <param name="app">Web应用程序。</param>
    public static void UseKnown(this WebApplication app)
    {
        if (CoreOption.Instance.IsCompression)
            app.UseResponseCompression();

        var provider = new FileExtensionContentTypeProvider();
        foreach (var item in CoreOption.Instance.ContentTypes)
        {
            provider.Mappings[item.Key] = item.Value;
        }

        app.UseStaticFiles();
        var webFiles = Config.GetUploadPath(true);
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            FileProvider = new PhysicalFileProvider(webFiles),
            RequestPath = "/Files"
        });
        var upload = Config.GetUploadPath();
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });

        //if (option.IsAddWebApi)
        //    app.UseMiddleware<WebApiMiddleware>();
        //    app.UseKnownWebApi();

        app.UseRouting();
        app.UseAntiforgery();
        app.MapControllers();
        app.MapRazorPages();
        Config.ServiceProvider = app.Services;
    }

    private static void AddDynamicWebApi(this IMvcBuilder builder)
    {
        builder.ConfigureApplicationPartManager(m =>
        {
            //m.ApplicationParts.Add(new AssemblyPart(Config.App.Assembly));
            foreach (var item in Config.Assemblies)
            {
                m.ApplicationParts.Add(new AssemblyPart(item));
            }
            m.FeatureProviders.Add(new ApiFeatureProvider());
        });

        builder.Services.Configure<MvcOptions>(o =>
        {
            o.Conventions.Add(new ApiConvention());
        });
    }

    private static void AddAuthProvider(this IServiceCollection services)
    {
        switch (CoreOption.Instance.AuthMode)
        {
            case AuthMode.Cookie:
                services.AddAuthentication().AddCookie(Constant.KeyAuth);
                services.AddScoped<IAuthStateProvider, CookieAuthStateProvider>();
                break;
            case AuthMode.Session:
                services.AddScoped<IAuthStateProvider, SessionAuthStateProvider>();
                break;
            case AuthMode.Identity:
                services.AddIdentityAuth();
                break;
            default:
                break;
        }
    }

    private static void AddIdentityAuth(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, IdentityAuthStateProvider>();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddIdentityCookies();
        services.AddIdentityCore<UserInfo>().AddSignInManager().AddDefaultTokenProviders();
        services.AddScoped<IUserStore<UserInfo>, UserStore>();
        services.AddScoped<IAuthStateProvider>(sp => (IdentityAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
    }

    private static void AddKnownServices(this IServiceCollection services)
    {
        var assembly = typeof(CoreOption).Assembly;
        Config.AddModule(assembly);
        CoreOption.Instance.AddAssembly(assembly);
        WeixinApi.Initialize(CoreOption.Instance.Weixin);
        Logger.Initialize(Config.App.WebLogDays);
        if (!Config.IsAdmin)
            Config.OnInitialModules = OnInitialModules;

        // 添加服务
        services.AddServices(assembly);
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));

        // 添加模型
        DbConfig.Models.Add<SysConfig>(x => new { x.AppId, x.ConfigKey });
        DbConfig.Models.Add<SysRoleModule>(x => new { x.RoleId, x.ModuleId });
        DbConfig.Models.Add<SysUserRole>(x => new { x.UserId, x.RoleId });

        // 注入后台任务
        TaskHelper.OnPendingTask = GetPendingTaskAsync;
        TaskHelper.OnSaveTask = SaveTaskAsync;
    }

    private static async Task<List<ModuleInfo>> OnInitialModules(Database db)
    {
        var modules = new List<ModuleInfo>();
        var items = await db.QueryListAsync<SysModule>();
        if (items != null && items.Count > 0)
        {
            foreach (var item in items.OrderBy(m => m.Sort))
            {
                modules.Add(item.ToModuleInfo());
            }
            DataHelper.Initialize(modules);
        }
        return modules;
    }

    private static async Task<TaskInfo> GetPendingTaskAsync(Database db, string type)
    {
        var info = await db.Query<SysTask>().Where(d => d.Status == TaskJobStatus.Pending && d.Type == type)
                           .OrderBy(d => d.CreateTime).FirstAsync<TaskInfo>();
        if (info != null)
        {
            db.User = await db.GetUserAsync(info.CreateBy);
            info.File = await db.Query<SysFile>().Where(d => d.Id == info.Target).FirstAsync<AttachInfo>();
        }
        return info;
    }

    private static Task SaveTaskAsync(Database db, TaskInfo info)
    {
        return db.SaveTaskAsync(info);
    }

    //private static void UseKnownWebApi(this IEndpointRouteBuilder app)
    //{
    //    foreach (var item in Config.ApiMethods)
    //    {
    //        if (item.HttpMethod == HttpMethod.Get)
    //            app.MapGet(item.Route, ctx => WebApi.Invoke(ctx, item));
    //        else
    //            app.MapPost(item.Route, ctx => WebApi.Invoke(ctx, item));
    //    }
    //}
}