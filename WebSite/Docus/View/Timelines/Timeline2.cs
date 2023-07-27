namespace WebSite.Docus.View.Timelines;

class Timeline2 : BaseComponent
{
    private readonly List<TimelineItem> items = new()
    {
        new TimelineItem { Title = "审核中", Type = StyleType.Info },
        new TimelineItem { Title = "发布成功", Type = StyleType.Success },
        new TimelineItem { Title = "审核失败", Type = StyleType.Danger }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Timeline>().Set(c => c.Items, items).Build();
    }
}