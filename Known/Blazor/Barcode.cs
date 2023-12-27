using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class Barcode : BaseComponent
{
    private string lastCode;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(Id).Class(Style).Close();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Value != lastCode)
        {
            lastCode = Value;
            JS.ShowBarcode(Id, Value, Option);
        }
        return base.OnAfterRenderAsync(firstRender);
    }
}