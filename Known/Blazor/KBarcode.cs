using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KBarcode : BaseComponent
{
    private string lastCode;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(Id).Class(Style).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Value != lastCode)
        {
            lastCode = Value;
            await JS.ShowBarcodeAsync(Id, Value, Option);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}