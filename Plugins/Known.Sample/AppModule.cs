using Known.Sample.Tests;

namespace Known.Sample;

public static class AppModule
{
    public static void AddSample(this IServiceCollection services)
    {
        Config.AddModule(typeof(AppModule).Assembly);

        var parentId = "0";
        if (Config.App.IsTopMenu)
        {
            parentId = "TopDemo";
            Config.Modules.AddItem("0", parentId, "业务", "block", 1);
            Config.Modules.AddItem("0", "TopHuman", "人事", "block", 2);
            Config.Modules.AddItem("0", "TopAsset", "资产", "block", 3);
            Config.Modules.AddItem("0", "TopFinance", "财务", "block", 4);
            Config.Modules.AddItem("0", "TopTax", "税务", "block", 5);
        }
        Config.Modules.AddItem(parentId, AppConstant.Demo, "示例页面", "block", 2);
        Config.Modules.AddItem(parentId, AppConstant.Test, "测试页面", "appstore", 3);
        Config.Modules.AddItem(parentId, AppConstant.Produce, "生产管理", "appstore-add", 4);
        Config.Modules.AddItem(parentId, AppConstant.Screen, "数据大屏", "desktop", 98);

        UIConfig.UserFormShowFooter = true;
        UIConfig.UserFormTabs.Set<UserDataForm>(2, "数据权限");

        KStyleSheet.AddStyle("_content/Known.Sample/css/web.css");
        KStyleSheet.AddStyle("_content/Known.Sample/css/screen.css");
        KScript.AddScript("_content/Known.Sample/js/china.js");
        KScript.AddScript("_content/Known.Sample/js/area_echarts.js");
        KScript.AddScript("_content/Known.Sample/js/js.js");
    }
}