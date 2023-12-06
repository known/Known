using Microsoft.Extensions.DependencyInjection;

namespace Known.Demo;

public static class Extension
{
    public static void AddDemoModule(this IServiceCollection services)
    {
        //添加数据字典类别
        Cache.AddDicCategory<AppDictionary>();

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }
}