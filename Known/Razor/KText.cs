namespace Known.Razor;

public class KText : Field
{
    private BaseRender<KText> render;

    public KText()
    {
        render = RenderFactory.Create<KText>();
    }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        render?.BuildTree(this, builder);
    }
}