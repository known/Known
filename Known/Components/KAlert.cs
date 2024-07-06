namespace Known.Components;

public class KAlert : BaseComponent
{
    [Parameter] public StyleType Type { get; set; }
    [Parameter] public string Text { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildAlert(builder, Text, Type);
    }
}