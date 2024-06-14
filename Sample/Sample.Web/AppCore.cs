namespace Sample.Web;

public static class AppCore
{
    public static void AddSampleCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        services.AddKnownCore(info =>
        {
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            action?.Invoke(info);
        });

        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IApplyService, ApplyService>();

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }
}