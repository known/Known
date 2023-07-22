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
                builder.Component<Banner>().Set(c => c.Content, b => b.Span("bold", "这里是默认横幅通知！")).Build();
                builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是主要横幅通知！")).Set(c => c.Style, StyleType.Primary).Build();
                builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是成功横幅通知！")).Set(c => c.Style, StyleType.Success).Build();
                builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是信息横幅通知！")).Set(c => c.Style, StyleType.Info).Build();
                builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是警告横幅通知！")).Set(c => c.Style, StyleType.Warning).Build();
                builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是危险横幅通知！")).Set(c => c.Style, StyleType.Danger).Build();
            });
        });
    }

    private void BuildNotify(RenderTreeBuilder builder)
    {
        builder.BuildDemo("通知", () =>
        {
            builder.Div(attr =>
            {
                builder.Button("默认", Callback(() => UI.Notify("<h1>这里是默认通知！</h1>")));
                builder.Button("主要", Callback(() => UI.Notify("这里是主要通知！", StyleType.Primary, 10000)), StyleType.Primary);
                builder.Button("成功", Callback(() => UI.Notify("这里是成功通知！", StyleType.Success)), StyleType.Success);
                builder.Button("信息", Callback(() => UI.Notify("这里是信息通知！", StyleType.Info)), StyleType.Info);
                builder.Button("警告", Callback(() => UI.Notify("这里是警告通知！", StyleType.Warning)), StyleType.Warning);
                builder.Button("危险", Callback(() => UI.Notify("这里是危险通知！", StyleType.Danger)), StyleType.Danger);
            });
        });
    }

    private void BuildToast(RenderTreeBuilder builder)
    {
        builder.BuildDemo("提示", () =>
        {
            builder.Div(attr =>
            {
                builder.Button("默认", Callback(() => UI.Toast("这里是默认提示！")));
                builder.Button("主要", Callback(() => UI.Toast("这里是主要提示！", StyleType.Primary)), StyleType.Primary);
                builder.Button("成功", Callback(() => UI.Toast("这里是成功提示！", StyleType.Success)), StyleType.Success);
                builder.Button("信息", Callback(() => UI.Toast("这里是信息提示！", StyleType.Info)), StyleType.Info);
                builder.Button("警告", Callback(() => UI.Toast("这里是警告提示！", StyleType.Warning)), StyleType.Warning);
                builder.Button("危险", Callback(() => UI.Toast("这里是危险提示！", StyleType.Danger)), StyleType.Danger);
            });
        });
    }
}