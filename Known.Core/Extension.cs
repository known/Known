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
        foreach (var item in Config.ApiMethods)
        {
            if (item.HttpMethod == HttpMethod.Get)
                app.MapGet(item.Route, ctx => InvokeMethod(ctx, item));
            else
                app.MapPost(item.Route, ctx => InvokeMethod(ctx, item));
        }
    }

    private static async Task InvokeMethod(HttpContext ctx, ApiMethodInfo info)
    {
        try
        {
            var token = ctx.Request.Headers[Constants.KeyToken].ToString();
            var service = ctx.RequestServices.GetRequiredService(info.MethodInfo.DeclaringType) as IService;
            service.Context = Context.Create(token);
            var parameters = new List<object>();
            foreach (var item in info.Parameters)
            {
                if (ctx.Request.Method == "GET")
                {
                    var value = ctx.Request.Query[item.Name].ToString();
                    var parameter = Utils.ConvertTo(item.ParameterType, value, null);
                    parameters.Add(parameter);
                }
                else
                {
                    var parameter = await ctx.Request.ReadFromJsonAsync(item.ParameterType);
                    parameters.Add(parameter);
                }
            }
            dynamic result = info.MethodInfo.Invoke(service, [.. parameters]);
            result.Wait();
            string text = Utils.ToJson(result.Result);
            await ctx.Response.WriteAsync(text);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            await ctx.Response.WriteAsJsonAsync(Result.Error(ex.Message));
        }
    }
}