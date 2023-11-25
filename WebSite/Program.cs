using WebSite;
using WebSite.Pages;

var builder = WebApplication.CreateBuilder(args);
AppConfig.RootPath = builder.Environment.ContentRootPath;
AppConfig.Initialize();
builder.Services.AddScoped(sp => new HttpClient());
// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();