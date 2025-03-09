using Sample.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();
builder.Services.AddApplication(option =>
{
    Config.IsDevelopment = builder.Configuration.GetSection("IsDevelopment").Get<bool>();
    option.App.WebRoot = builder.Environment.WebRootPath;
    option.App.ContentRoot = builder.Environment.ContentRootPath;
    option.Database = db =>
    {
        var connString = builder.Configuration.GetSection("ConnString").Get<string>();
        //db.AddAccess<System.Data.OleDb.OleDbFactory>(connString);
        db.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(connString);
        //db.AddSqlServer<Microsoft.Data.SqlClient.SqlClientFactory>(connString);
        //db.AddSqlServer<Oracle.ManagedDataAccess.Client.OracleClientFactory>(connString);
        //db.AddMySql<MySqlConnector.MySqlConnectorFactory>(connString);
        //db.AddPgSql<Npgsql.NpgsqlFactory>(connString);
        //db.AddDM<Dm.DmClientFactory>(connString);
        //db.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
        //db.OperateMonitors.Add(info => Console.WriteLine(info.ToString()));
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseApplication();
app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AddAdditionalAssemblies([.. Config.Assemblies]);
app.Run();
