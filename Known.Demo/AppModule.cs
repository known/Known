using Known.Demo.Pages;
using Known.Demo.Pages.BizApply;

namespace Known.Demo;

public static class AppModule
{
    public static void AddDemo(this IServiceCollection services)
    {
        var assembly = typeof(AppModule).Assembly;
        //添加模块程序集
        //框架模块管理配置自动反射Entity和Model
        Config.AddModule(assembly);
        //注入项目数据字典类别
        DicCategory.AddCategories<AppDictionary>();

        //配置默认首页
        Config.Home = new KMenuItem("首页", "fa fa-home", typeof(Home));
        //注册待办事项显示流程表单
        Config.ShowMyFlow = flow =>
        {
            if (flow.Flow.FlowCode == AppFlow.Apply.Code)
                ApplyForm.ShowMyFlow(flow);
        };
    }
}