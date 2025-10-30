using Known;
using Known.Data;

namespace Sample;

public static class AppServer
{
    //private static readonly List<MenuInfo> AppMenus =
    //[
    //    new MenuInfo { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
    //    new MenuInfo { Id = "App", Name = "应用", Icon = "appstore", Target = "Tab", Url = "/app/application" },
    //    new MenuInfo { Id = "Disc", Name = "发现", Icon = "compass", Target = "Tab", Url = "/app/discovery" },
    //    new MenuInfo { Id = "Test", Name = "收货", Icon = "import", Target = "Menu", Color = "#1464ad", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "上架", Icon = "vertical-align-top", Target = "Menu", Color = "#2db7f5", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "下架", Icon = "vertical-align-bottom", Target = "Menu", Color = "#722ed1", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "盘点", Icon = "schedule", Target = "Menu", Color = "#108ee9", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "移库", Icon = "merge-cells", Target = "Menu", Color = "#f50", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "库存", Icon = "insert-row-above", Target = "Menu", Color = "#fa8c16", Url = "/app/test" },
    //    new MenuInfo { Id = "Test", Name = "波次", Icon = "partition", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
    //    new MenuInfo { Id = "Add", Name = "功能待加", Icon = "plus", Target = "Menu", Color = "#4fa624" }
    //];

    // 添加系统 Web 后端。
    internal static void AddApplicationWeb(this IServiceCollection services, Action<CoreOption> action)
    {
        CoreConfig.OnInitial = OnInitial;
        services.AddApplication();
        services.AddKnownWeb(action);
        services.AddHostedService<TestWorker>();
    }

    internal static void UseApplication(this WebApplication app)
    {
        app.UseKnown();
    }

    private static async Task OnInitial(Database db, InitialInfo info)
    {
        //info.ClientHomes["KC-799f9735efe8468fa290d5486ca7dc6c"] = "/pda";
    }
}