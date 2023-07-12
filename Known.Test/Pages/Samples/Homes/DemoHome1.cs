using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Homes;

class DemoHome1 : BaseComponent
{
    private readonly List<TimelineItem> items1 = new()
    {
        new TimelineItem{Title="第一标题",Description="第一节点内容"},
        new TimelineItem{Title="第二标题",Description="第二节点内容"},
        new TimelineItem{Title="第三标题",Description="第三节点内容"}
    };
    private readonly List<TimelineItem> items2 = new()
    {
        new TimelineItem{Title="审核中",Type=StyleType.Info},
        new TimelineItem{Title="发布成功",Type=StyleType.Success},
        new TimelineItem{Title="审核失败",Type=StyleType.Danger}
    };
    private readonly List<TimelineItem> items3 = new()
    {
        new TimelineItem{Title="第一标题",Description="第一节点内容"},
        new TimelineItem{Template=BuildTimelineItem},
        new TimelineItem{Title="第三标题",Description="第三节点内容"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("row", attr =>
        {
            builder.Component<Timeline>()
                   .Set(c => c.Items, items1)
                   .Build();
            builder.Component<Timeline>()
                   .Set(c => c.Items, items2)
                   .Build();
            builder.Component<Timeline>()
                   .Set(c => c.Items, items3)
                   .Build();
        });
    }

    private static void BuildTimelineItem(RenderTreeBuilder builder)
    {
        builder.Span("name", "自定义节点");
        builder.Span("time", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.Span("text", "自定义模板内容");
        builder.Img("/img/login.jpg");
    }
}