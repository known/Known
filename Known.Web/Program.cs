using Known;
using Known.Cells;
using Known.Demo;
using Known.Extensions;
using Known.Web.Pages;
using KnownAntDesign;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
//添加Known框架
builder.Services.AddKnown(info =>
{
    //设置环境
    info.WebRoot = builder.Environment.WebRootPath;
    info.ContentRoot = builder.Environment.ContentRootPath;
    info.IsDevelopment = builder.Environment.IsDevelopment();
    //项目ID、名称、类型、程序集
    info.Id = "KIMS";
    info.Name = "Known信息管理系统";
    info.Type = AppType.Web;
    info.Assembly = typeof(App).Assembly;
    //数据库连接
    info.Connections = [new Known.ConnectionInfo
    {
        Name = "Default",
        DatabaseType = DatabaseType.SQLite,
        ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
        ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
    }];
    //上传文件路径
    info.UploadPath = builder.Configuration.GetSection("UploadPath").Get<string>();
    //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
    info.JsPath = "/script.js";

    //设置产品ID，根据硬件获取ID
    info.ProductId = $"{Config.App.Id}-000001";
});
//添加默认Excel实现
builder.Services.AddKnownCells();
//添加KnownAntDesign页面
builder.Services.AddKnownAntDesign(option =>
{
    //添加页脚内容
    option.Footer = b => b.Span($"{Config.App.Id} ©2023-{DateTime.Now:yyyy} Created by Known");
});
//添加Demo模块
builder.Services.AddDemo();

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