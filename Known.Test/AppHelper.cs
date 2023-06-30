using Known.Test.Pages;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Known.Test;

class AppHelper
{
    internal const string Host = "http://localhost:5000";

    internal static void Run()
    {
        InitDatabase();
        InitConfig();
        Task.Run(() => CreateWebHostBuilder(Array.Empty<string>()).Build().Run());
        Application.Run(new MainForm());
    }

    private static void InitDatabase()
    {
        var fileName = "Test.db";
        var path = Path.Combine(Application.StartupPath, fileName);
        if (!File.Exists(path))
        {
            var assembly = typeof(AppHelper).Assembly;
            var names = assembly.GetManifestResourceNames();
            var name = names.FirstOrDefault(n => n.Contains(fileName));
            if (string.IsNullOrWhiteSpace(name))
                return;

            Utils.EnsureFile(path);
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var fs = File.Create(path))
            {
                stream?.CopyTo(fs);
            }
        }
    }

    private static void InitConfig()
    {
        DicCategory.AddCategories<AppDictionary>();

        Config.IsPlatform = true;
        Config.SetAppAssembly(typeof(AppHelper).Assembly);
        
        KRConfig.Home = new MenuItem("首页", "fa fa-home", typeof(Home));
        
        KCConfig.AddWebPlatform();
        KCConfig.WebRoot = Application.StartupPath;
        KCConfig.ContentRoot = Application.StartupPath;

        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
        });
        var connInfo = new ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = $"Data Source=Test.db;"
        };
        KCConfig.App = new AppInfo
        {
            Connections = new List<ConnectionInfo> { connInfo }
        };
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
                      .UseUrls(Host)
                      .UseStartup<Startup>();
    }
}

class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddControllers();
        builder.ConfigureApplicationPartManager(apm =>
        {
            apm.ApplicationParts.Add(new AssemblyPart(typeof(BaseController).Assembly));
            apm.ApplicationParts.Add(new AssemblyPart(typeof(Startup).Assembly));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        var upload = KCConfig.GetUploadPath();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public class AppDictionary
{
    public const string Test = "测试";
    public const string Type = "类型";
}