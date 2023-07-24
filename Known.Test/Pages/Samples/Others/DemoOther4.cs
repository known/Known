namespace Known.Test.Pages.Samples.Others;

/// <summary>
/// 反馈类
/// </summary>
class DemoOther4 : BaseComponent
{
    //Toast、Notify、Banner、Progress

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBanner(builder);
        BuildNotify(builder);
        BuildToast(builder);
    }

    private static void BuildBanner(RenderTreeBuilder builder)
    {
        builder.BuildDemo("横幅通知", () =>
        {
            builder.Div(attr =>
            {
                builder.Banner(StyleType.Primary, b => b.Span("fa fa-check bold", "这里是自定义横幅通知！"));
                builder.Banner(StyleType.Default, "这里是默认类横幅通知！");
                builder.Banner(StyleType.Primary, "这里是主要类横幅通知！");
                builder.Banner(StyleType.Success, "这里是成功类横幅通知！");
                builder.Banner(StyleType.Info, "这里是信息类横幅通知！");
                builder.Banner(StyleType.Warning, "这里是警告类横幅通知！");
                builder.Banner(StyleType.Danger, "这里是危险类横幅通知！");
            });
        });
    }

    private void BuildNotify(RenderTreeBuilder builder)
    {
        builder.BuildDemo("通知", () =>
        {
            builder.Div(attr =>
            {
                builder.Button("默认", Callback(() => UI.Notify("<h1>这里是默认类通知！</h1>", StyleType.Default)));
                builder.Button("主要", Callback(() => UI.Notify("这里是主要类通知！", StyleType.Primary, 10000)), StyleType.Primary);
                builder.Button("成功", Callback(() => UI.Notify("这里是成功类通知！", StyleType.Success)), StyleType.Success);
                builder.Button("信息", Callback(() => UI.Notify("这里是信息类通知！", StyleType.Info)), StyleType.Info);
                builder.Button("警告", Callback(() => UI.Notify("这里是警告类通知！", StyleType.Warning)), StyleType.Warning);
                builder.Button("危险", Callback(() => UI.Notify("这里是危险类通知！", StyleType.Danger)), StyleType.Danger);
            });
        });
    }

    private void BuildToast(RenderTreeBuilder builder)
    {
        builder.BuildDemo("提示", () =>
        {
            builder.Div(attr =>
            {
                builder.Button("默认", Callback(() => UI.Toast("<h1>这里是默认类提示！</h1>", StyleType.Default)));
                builder.Button("主要", Callback(() => UI.Toast("这里是主要类提示！", StyleType.Primary)), StyleType.Primary);
                builder.Button("成功", Callback(() => UI.Toast("这里是成功类提示！", StyleType.Success)), StyleType.Success);
                builder.Button("信息", Callback(() => UI.Toast("这里是信息类提示！", StyleType.Info)), StyleType.Info);
                builder.Button("警告", Callback(() => UI.Toast("这里是警告类提示！", StyleType.Warning)), StyleType.Warning);
                builder.Button("危险", Callback(() => UI.Toast("这里是危险类提示！", StyleType.Danger)), StyleType.Danger);
            });
        });
    }
}