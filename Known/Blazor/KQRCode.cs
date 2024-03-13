using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KQRCode : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(Id).Class(Style).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.ShowQRCodeAsync(Id, Option);
        await base.OnAfterRenderAsync(firstRender);
    }
}