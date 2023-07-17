namespace Known.Razor.Components;

public class QRCode : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public object Option { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div(Style, attr => attr.Id(Id));
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            UI.ShowQRCode(Id, Option);
        }
        return base.OnAfterRenderAsync(firstRender);
    }
}