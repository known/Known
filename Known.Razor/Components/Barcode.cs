namespace Known.Razor.Components;

public class Barcode : BaseComponent
{
    private string lastCode;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Canvas(attr => attr.Id(Id).Class(Style));
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Value != lastCode)
        {
            lastCode = Value;
            UI.ShowBarcode(Id, Value, Option);
        }
        return base.OnAfterRenderAsync(firstRender);
    }
}