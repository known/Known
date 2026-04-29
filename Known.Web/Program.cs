using Known.Web;
using Known.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddSingleton<DocumentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapGet("/sitemap.xml", (DocumentService service) =>
{
    return Results.Text(SitemapService.BuildXml(service), "application/xml", System.Text.Encoding.UTF8);
});
app.MapRazorComponents<App>();

app.Run();
