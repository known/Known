using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Homes;

class DemoHome1 : BaseComponent
{
    private readonly List<DmGoods> items = new()
    {
        new DmGoods{Name="第一标题",Model="第一节点内容"},
        new DmGoods{Name="第二标题",Model="第二节点内容"},
        new DmGoods{Name="第三标题",Model="第三节点内容"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Timeline<DmGoods>>()
               .Set(c => c.Items, items)
               .Set(c => c.Template, BuildTimelineItem)
               .Build();
    }

    private void BuildTimelineItem(RenderTreeBuilder builder, DmGoods item)
    {
        builder.Span("name", item.Name);
        builder.Span("time", $"{item.CreateTime:yyyy-MM-dd HH:mm:ss}");
        builder.Span("text", item.Model);
    }
}