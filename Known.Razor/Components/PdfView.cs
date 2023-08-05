namespace Known.Razor.Components;

public class PdfView : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public Stream Stream { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div(Style, attr => attr.Id(Id));
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        UI.ShowPdf(Id, Stream);
        return base.OnAfterRenderAsync(firstRender);
    }
}