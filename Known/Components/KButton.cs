namespace Known.Components;

public class KButton : BaseComponent
{
    [Parameter] public ActionInfo Action { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildButton(builder, Action);
    }
}