namespace Known.Razor;

public class KButton : BaseComponent
{
    private BaseRender<KButton> render;

    public KButton()
    {
        Id = Utils.GetGuid();
        render = RenderFactory.Create<KButton>();
    }

    [Parameter] public StyleType Type { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        render?.BuildTree(this, builder);
    }
}