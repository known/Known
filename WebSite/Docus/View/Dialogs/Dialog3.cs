using WebSite.Docus.View.Charts;
using WebSite.Docus.View.Timelines;

namespace WebSite.Docus.View.Dialogs;

class Dialog3 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("弹出柱状图组件", Callback(OnChartBar), StyleType.Primary);
        builder.Button("弹出自定义", Callback(OnCustom), StyleType.Primary);
    }

    private void OnChartBar()
    {
        UI.Show<Chart2>("柱状图", new(600, 400));
    }

    private void OnCustom()
    {
        UI.Show(new DialogOption
        {
            Title = "自定义",
            Size = new(600, 400),
            Content = BuildContent
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Div("demo-dialog", attr =>
        {
            builder.Div("title", "流程状态");
            builder.Component<Timeline2>().Build();
        });
    }
}