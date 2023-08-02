namespace WebSite.Docus.View.GroupBoxs;

class GroupBox1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<GroupBox>()
               .Set(c => c.Title, "基本信息")
               .Set(c => c.ChildContent, BuildBody)
               .Build();
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Span("这里是GroupBox内容");
    }
}