using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Template.WinForm;

public class AppHelper
{
    public static void Run()
    {
        AppAlone.Initialize();
        Task.Run(() => CreateWebHostBuilder(Array.Empty<string>()).Build().Run());
        Application.Run(new MainForm());
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
                      .UseUrls(AppAlone.Host)
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
            apm.ApplicationParts.Add(new AssemblyPart(typeof(HomeController).Assembly));
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