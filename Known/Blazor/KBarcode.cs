using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KBarcode : BaseComponent
{
    private readonly string id;
    private string lastCode;

    public KBarcode()
    {
        id = Utils.GetGuid();
        id = $"bc-{id}";
    }

    [Parameter] public string Style { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(id).Class(Style).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Value != lastCode)
        {
            lastCode = Value;
            await JS.ShowBarcodeAsync(id, Value, Option);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}