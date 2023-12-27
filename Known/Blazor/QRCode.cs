using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class QRCode : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div().Id(Id).Class(Style).Close();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            JS.ShowQRCode(Id, Option);
        return base.OnAfterRenderAsync(firstRender);
    }
}