using Sample.Web;
//using Toolbelt.Extensions.DependencyInjection;

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
    //数据库连接
    info.Connections = [new Known.ConnectionInfo
    {
        Name = "Default",
        DatabaseType = DatabaseType.SQLite,
        ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
        //DatabaseType = DatabaseType.Access,
        //ProviderType = typeof(System.Data.OleDb.OleDbFactory),
        //DatabaseType = DatabaseType.MySql,
        //ProviderType = typeof(MySqlConnector.MySqlConnectorFactory),
        //DatabaseType = DatabaseType.Npgsql,
        //ProviderType = typeof(Npgsql.NpgsqlFactory),
        //DatabaseType = DatabaseType.SqlServer,
        //ProviderType = typeof(System.Data.SqlClient.SqlClientFactory),
        //ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
    }];
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
app.MapRazorComponents<App>()   
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies([.. Config.Assemblies]);
app.Run();