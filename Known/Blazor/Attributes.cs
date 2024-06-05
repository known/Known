namespace Known.Blazor;

public class AutoRenderModeAttribute : RenderModeAttribute
{
    private static IComponentRenderMode ModeImpl => RenderMode.InteractiveAuto;

    public override IComponentRenderMode Mode => ModeImpl;
}