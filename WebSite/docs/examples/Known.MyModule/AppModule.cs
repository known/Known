namespace Known.MyModule;

public static class AppModule
{
    public static void AddMyModule(this IServiceCollection services)
    {
        Config.AddModule(typeof(AppModule).Assembly);

        var parentId = "0";
        if (Config.App.IsTopMenu)
        {
            parentId = "TopDemo";
            Config.Modules.AddItem("0", parentId, "业务", "block", 1);
        }

        Config.Modules.AddItem(parentId, "MyModule", "示例模块", "appstore", 20);
        KStyleSheet.AddStyle("_content/Known.MyModule/css/web.css");
    }
}
