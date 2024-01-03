using Known.Shared;

#if DEBUG
Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddHubOptions(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                });
builder.Services.AddApp(info =>
{
    //设置环境
    info.WebRoot = builder.Environment.WebRootPath;
    info.ContentRoot = builder.Environment.ContentRootPath;
    info.IsDevelopment = builder.Environment.IsDevelopment();
    info.Connections[0].ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>();
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
app.UseApp();
app.MapRazorComponents<Known.Web.App>()
   .AddInteractiveServerRenderMode();
app.Run();