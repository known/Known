namespace WebSite.Docus.Feedback.Progress;

class Progress2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Progress(StyleType.Primary, 0.35M, 100);
        builder.Progress(StyleType.Success, 0.6M, 200);
    }
}