namespace Known.Razor;

public class FlowChart : PageComponent
{
    [Parameter] public FlowInfo Info { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Info == null)
            return;

        await UI.ShowFlowChart(Info);
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("flowChart", attr =>
        {
            //builder.Div("title", Info?.Name);
            builder.Div(attr => attr.Id(Info?.Id));
        });
    }
}