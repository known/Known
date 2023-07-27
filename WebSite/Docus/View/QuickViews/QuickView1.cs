namespace WebSite.Docus.View.QuickViews;

class QuickView1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("显示", Callback(OnShow), StyleType.Primary);
        builder.Component<QuickView>().Id("qvTest")
               .Set(c => c.ChildContent, BuildContent)
               .Build();
    }

    private void OnShow() => UI.ShowQuickView("qvTest");

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Span("这里是快速预览");
    }
}