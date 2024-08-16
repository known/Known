using Known.SqlSugar;
using Sample.Web;
using SqlSugar;

#if DEBUG
Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
builder.Services.AddApp(info =>
{
    info.WebRoot = builder.Environment.WebRootPath;
    info.ContentRoot = builder.Environment.ContentRootPath;
});
builder.Services.AddKnownSqlSugar(config =>
{
    config.DbType = DbType.MySql;
    config.ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>();
    config.IsAutoCloseConnection = true;
    config.MoreSettings ??= new ConnMoreSettings();
    config.MoreSettings.IsAutoToUpper = false;
    config.AopEvents ??= new AopEvents();
    config.AopEvents.OnLogExecuting = (sql, pars) =>
    {
        var param = string.Join(",", pars.Select(p => $"{p.ParameterName}={p.Value}"));
        Console.WriteLine($"SQL: {sql}");
        Console.WriteLine($"参数: {param}");
    };
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    //app.UseCssLiveReload();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();
app.UseApp();
app.MapRazorPages();
app.MapRazorComponents<SqlSugarWeb.App>()   
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies([.. Config.Assemblies]);
app.Run();