using Known.Sample.Tests;

namespace Known.Sample;

public static class AppModule
{
    public static void AddSample(this IServiceCollection services)
    {
        Config.AddModule(typeof(AppModule).Assembly);
        Config.Modules.AddItem("0", AppConstant.Demo, "示例页面", "block", 2);

        UIConfig.UserFormShowFooter = true;
        UIConfig.UserFormTabs.Set<UserDataForm>(2, "数据权限");

        KStyleSheet.AddStyle("_content/Known.Sample/css/web.css");
        KStyleSheet.AddStyle("_content/Known.Sample/css/screen.css");
        KScript.AddScript("_content/Known.Sample/js/china.js");
        KScript.AddScript("_content/Known.Sample/js/area_echarts.js");
        KScript.AddScript("_content/Known.Sample/js/js.js");
    }
}