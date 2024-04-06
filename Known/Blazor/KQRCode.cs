using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KQRCode : BaseComponent
{
    private readonly string id;

    public KQRCode()
    {
        id = Utils.GetGuid();
        id = $"qr-{id}";
    }

    [Parameter] public string Style { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(id).Class(Style).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.ShowQRCodeAsync(id, Option);
        await base.OnAfterRenderAsync(firstRender);
    }
}