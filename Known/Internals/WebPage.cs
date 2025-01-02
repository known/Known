namespace Known.Internals;

class WebPage : BaseComponent
{
    [Parameter] public PageModel Model { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model.StateChanged = StateChanged;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model.Items == null || Model.Items.Count == 0)
            return;

        if (Model.Type == PageType.None)
        {
            BuildItems(builder);
            return;
        }

        var className = Model.Type == PageType.Column ? $"kui-row-{Model.Spans}" : Model.Custom;
        builder.Div(className, () => BuildItems(builder));
    }

    private void BuildItems(RenderTreeBuilder builder)
    {
        foreach (var item in Model.Items)
        {
            if (!string.IsNullOrWhiteSpace(item.ClassName))
                builder.Div(item.ClassName, () => builder.Fragment(item.Content));
            else
                builder.Fragment(item.Content);
        }
    }
}