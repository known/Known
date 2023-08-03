using System.IO.Compression;
using Known.Core.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Core.Extensions;

public static class WebAppExtension
{
    public static void RunAsWebApi(this WebApplicationBuilder builder, Action<IServiceCollection> action = null)
    {
        KCConfig.AddWebPlatform();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<AuthActionFilter>();
        });
        action?.Invoke(builder.Services);

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    public static void RunAsBlazorWebAssembly(this WebApplicationBuilder builder, Action<IServiceCollection> serviceAction, Action<WebApplication> appAction)
    {
        KCConfig.AddWebPlatform();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<AuthActionFilter>();
        });
        builder.Services.AddRazorPages();
        AddCompression(builder.Services);
        BuildHostUrl(builder);
        serviceAction?.Invoke(builder.Services);

        var app = builder.Build();
        UseWebApp(app);
        appAction?.Invoke(app);
        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");
        app.Run();
    }

    public static void RunAsBlazorServer(this WebApplicationBuilder builder, Action<IServiceCollection> action)
    {
        KCConfig.AddWebPlatform();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthenticationCore();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor().AddHubOptions(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
            options.EnableDetailedErrors = false;
            options.HandshakeTimeout = TimeSpan.FromSeconds(60);
            options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            options.MaximumParallelInvocationsPerClient = 1;
            options.MaximumReceiveMessageSize = 1024 * 1024;
            options.StreamBufferCapacity = 18;
        });
        builder.Services.AddScoped<ProtectedSessionStorage>();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        AddCompression(builder.Services);
        BuildHostUrl(builder);
        action?.Invoke(builder.Services);

        var app = builder.Build();
        UseWebApp(app);
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.Run();
    }

    private static void AddCompression(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        services.Configure<BrotliCompressionProviderOptions>(config =>
        {
            config.Level = CompressionLevel.Fastest;
        });
        services.Configure<GzipCompressionProviderOptions>(config =>
        {
            config.Level = CompressionLevel.Fastest;
        });
    }

    private static void BuildHostUrl(WebApplicationBuilder builder)
    {
        var httpPort = builder.Configuration.GetSection("HttpPort").Get<int>();
        if (httpPort == 0)
            httpPort = 5000;

        var httpsPort = builder.Configuration.GetSection("HttpsPort").Get<int>();
        if (httpsPort > 0)
            builder.WebHost.UseUrls($"http://*:{httpPort}", $"https://*:{httpsPort}");
        else
            builder.WebHost.UseUrls($"http://*:{httpPort}");
    }

    private static void UseWebApp(WebApplication app)
    {
        KCConfig.IsDevelopment = app.Environment.IsDevelopment();
        // Configure the HTTP request pipeline.
        if (!KCConfig.IsDevelopment)
        {
            app.UseResponseCompression();
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        var upload = KCConfig.GetUploadPath();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });

        app.UseRouting();
    }
}