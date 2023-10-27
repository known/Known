using Known;
using Known.Cells;
using Known.Demo;
using Known.Web;
using Known.Web.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.InitApp();                //初始化配置

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddKnown();      //添加Known框架
builder.Services.AddKnownCells(); //添加Known.Cells处理Excel
builder.Services.AddApp();        //添加APP全局设置
builder.Services.AddDemo();       //添加APP的Demo模块

var app = builder.Build();
app.Services.UseApp();            //使用APP

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