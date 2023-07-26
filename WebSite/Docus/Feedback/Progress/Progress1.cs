namespace WebSite.Docus.Feedback.Progress;

class Progress1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Progress(StyleType.Default, 0.5M);
        builder.Progress(StyleType.Primary, 0.35M);
        builder.Progress(StyleType.Success, 1);
        builder.Progress(StyleType.Info, 0.6M);
        builder.Progress(StyleType.Warning, 0.55M);
        builder.Progress(StyleType.Danger, 0.8M);
    }
}