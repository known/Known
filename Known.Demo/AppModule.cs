namespace Known.Demo;

public static class AppModule
{
    private static readonly List<MenuInfo> AppMenus =
    [
        new MenuItem { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
        new MenuItem { Id = "Mine", Name = "我的", Icon = "user", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Test", Name = "测试模块", Icon = "form", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
        new MenuInfo { Id = "Add", Name = "功能待加", Icon = "plus", Target = "Menu", Color = "#4fa624" }
    ];

    public static void AddDemo(this IServiceCollection services)
    {
        Config.AppMenus = AppMenus;
        //添加模块
        Config.AddModule(typeof(AppModule).Assembly);

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }
}