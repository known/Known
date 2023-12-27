using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class PdfView : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public Stream Stream { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div().Id(Id).Class(Style).Close();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        JS.ShowPdf(Id, Stream);
        return base.OnAfterRenderAsync(firstRender);
    }
}