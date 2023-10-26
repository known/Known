using Known.Demo.Pages;
using Known.Demo.Pages.BizApply;

namespace Known.Demo;

public static class AppModule
{
    public static void AddDemo(this IServiceCollection services)
    {
        var assembly = typeof(AppModule).Assembly;
        //添加模块
        Config.Modules.Add(assembly);
        //注入项目程序集
        //框架关于系统自动获取软件版本号
        //框架模块管理配置自动反射Entity和Model
        Config.SetAppAssembly(assembly);
        //附加项目CodeTable特性类字典到缓存
        Cache.AttachCodes(assembly);
        
        //配置默认首页
        KRConfig.Home = new KMenuItem("首页", "fa fa-home", typeof(Home));
        //注册待办事项显示流程表单
        KRConfig.ShowMyFlow = flow =>
        {
            if (flow.Flow.FlowCode == AppFlow.Apply.Code)
                ApplyForm.ShowMyFlow(flow);
        };
    }
}