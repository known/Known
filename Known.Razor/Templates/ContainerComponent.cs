using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Known.Razor.Templates;

[SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
class ContainerComponent : IComponent
{
    private readonly HtmlRenderer renderer;
    private readonly int componentId;
    private RenderHandle renderHandle;

    public ContainerComponent(HtmlRenderer renderer)
    {
        this.renderer = renderer;
        componentId = renderer.AttachTestRootComponent(this);
    }

    public void Attach(RenderHandle renderHandle)
    {
        this.renderHandle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        throw new NotImplementedException($"{nameof(ContainerComponent)} shouldn't receive any parameters");
    }

    public (int, object) FindComponentUnderTest()
    {
        var ownFrames = renderer.GetCurrentRenderTreeFrames(componentId);
        if (ownFrames.Count == 0)
            throw new InvalidOperationException($"{nameof(ContainerComponent)} hasn't yet rendered");

        ref var childFrame = ref ownFrames.Array[0];
        Debug.Assert(childFrame.FrameType == RenderTreeFrameType.Component);
        Debug.Assert(childFrame.Component != null);
        return (childFrame.ComponentId, childFrame.Component);
    }

    public void RenderComponentUnderTest(Type componentType, ParameterView parameters)
    {
        renderer.DispatchAndAssertNoSynchronousErrors(() =>
        {
            renderHandle.Render(builder =>
            {
                builder.OpenComponent(0, componentType);

                foreach (var parameterValue in parameters)
                {
                    builder.AddAttribute(1, parameterValue.Name, parameterValue.Value);
                }

                builder.CloseComponent();
            });
        });
    }
}