namespace Sample.Client.Pages;

class CustomTablePage : BaseComponent
{
    [Parameter] public TableModel<Dictionary<string, object>> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        builder.Span(Model.PageName);
    }
}