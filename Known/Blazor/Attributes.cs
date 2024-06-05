namespace Known.Blazor;

public class ServerRenderModeAttribute : RenderModeAttribute
{
    private static IComponentRenderMode ModeImpl => RenderMode.InteractiveServer;

    public override IComponentRenderMode Mode => ModeImpl;
}

public class AutoRenderModeAttribute : RenderModeAttribute
{
    private static IComponentRenderMode ModeImpl => RenderMode.InteractiveAuto;

    public override IComponentRenderMode Mode => ModeImpl;
}