using Template.Server;

var builder = WebApplication.CreateBuilder(args);
AppServer.Initialize(builder);

builder.RunAsBlazorWebAssembly(services =>
{
    services.AddApp();
}, app =>
{
    if (KCConfig.IsDevelopment)
        app.UseWebAssemblyDebugging();
    app.UseBlazorFrameworkFiles();
    app.UseApp();
});