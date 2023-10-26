namespace WebSite.Docus.View.Emptys;

class Empty1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KEmpty>()
               .Set(c => c.Text, "模块建设中...")
               .Build();
    }
}