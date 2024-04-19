namespace Known.Demo;

public static class Extension
{
    public static void AddDemoModule(this IServiceCollection services)
    {
        //添加模块
        Config.AddModule(typeof(Extension).Assembly);

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }
}