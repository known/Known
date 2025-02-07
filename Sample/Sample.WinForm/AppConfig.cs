namespace Sample.WinForm;

public static class AppConfig
{
    public static string AppId => "KIMS";
    public static string AppName => "Known信息管理系统";

    public static void AddApplication(this IServiceCollection services)
    {
        Console.WriteLine(AppName);
#if DEBUG
        Config.IsDevelopment = true;
        Config.IsDebug = true;
#endif
        Config.Modules.Add(AppConstant.Demo, "示例页面", "block", "0", 2);

        var assembly = typeof(AppConfig).Assembly;
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
            info.NextIdType = NextIdType.Snowflake;
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            //info.JsPath = "./script.js";
        });
        services.AddKnownCore();

        //UIConfig.AutoTablePage = (b, m) => b.Component<CustomTablePage>().Set(c => c.Model, m).Build();
        UIConfig.Errors["403"] = new ErrorConfigInfo { Description = "你没有此页面的访问权限。" };

        // 注入服务
        services.AddScoped<IHomeService, HomeService>();
    }
}