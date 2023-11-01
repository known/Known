namespace Known.Razor;

public class KPassword : Field
{
    private BaseRender<KPassword> render;

    public KPassword()
    {
        render = RenderFactory.Create<KPassword>();
    }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        render?.BuildTree(this, builder);
    }
}