using Known;
using Known.Cells;
using Known.Demo;
using Known.Web;
using Known.Web.Pages;

var builder = WebApplication.CreateBuilder(args);
AppWeb.Initialize(builder);
builder.Services.AddKnown();
builder.Services.AddKnownCells();
builder.Services.AddKnownWeb();
builder.Services.AddDemo();

var app = builder.Build();
app.Services.UseKnownWeb();

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