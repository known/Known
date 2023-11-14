using Known;
using Known.Cells;
using Known.Demo;
using Known.Web;
using Known.Web.Pages;
using KnownAntDesign;

var builder = WebApplication.CreateBuilder(args);
builder.InitApp();
// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
builder.Services.AddKnown();      //添加Known框架
builder.Services.AddKnownCells(); //添加默认Excel实现
builder.Services.AddAntDesign();  //添加AntDesign
builder.Services.AddKAntDesign(); //添加KnownAntDesign页面
builder.Services.AddDemo();       //添加Demo模块

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