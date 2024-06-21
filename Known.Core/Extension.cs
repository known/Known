namespace Known.Core;

public static class Extension
{
    private static bool IsAddWebApi { get; set; }

    public static void AddKnownWin(this IServiceCollection services)
    {
        services.AddResponseCompression();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        //services.AddAuthorizationCore();
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
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<IAuthStateProvider, WebAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, WebAuthStateProvider>();
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
        foreach (var item in Config.ApiMethods)
        {
            //Console.WriteLine(item.Route);
            if (item.HttpMethod == HttpMethod.Get)
                app.MapGet(item.Route, ctx => InvokeMethod(ctx, item.MethodInfo));
            else
                app.MapPost(item.Route, ctx => InvokeMethod(ctx, item.MethodInfo));
        }
    }

    private static async Task InvokeMethod(HttpContext ctx, MethodInfo method)
    {
        try
        {
            var token = ctx.Request.Headers[Constants.KeyToken].ToString();
            var service = ctx.RequestServices.GetRequiredService(method.DeclaringType) as IService;
            service.Context = Context.Create(token);
            var parameters = new List<object>();
            foreach (var item in method.GetParameters())
            {
                var parameter = ctx.Request.Query[item.Name].ToString();
                parameters.Add(parameter);
            }
            var value = method.Invoke(service, [.. parameters]);
            var task = Utils.MapTo<TaskInfo>(value);
            await ctx.Response.WriteAsJsonAsync(task.Result);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            await ctx.Response.WriteAsJsonAsync(Result.Error(ex.Message));
        }
    }
}

public class TaskInfo
{
    public int Id { get; set; }
    public object Result { get; set; }
}