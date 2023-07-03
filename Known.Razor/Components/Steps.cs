namespace Known.Razor.Components;

public class Steps : Tabs
{
    private MenuItem curItem;
    private string PositionCss => Position.ToString().ToLower();

    protected override Task OnInitializedAsync()
    {
        curItem = Items?.First();
        return base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Count == 0)
            return;

        var css = CssBuilder.Default("tabs").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            BuildStepHead(builder);
            BuildStepBody(builder);
        });
    }

    private void BuildStepHead(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("tab").AddClass(PositionCss).Build();
        builder.Ul(css, attr =>
        {
            foreach (var item in Items)
            {
                builder.Li(curItem == item ? "active" : "", attr =>
                {
                    attr.OnClick(Callback(e => OnItemClick(item)));
                    builder.IconName(item.Icon, item.Name);
                });
            }
        });
    }

    private void BuildStepBody(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var css = CssBuilder.Default("tab-body")
                                .AddClass(PositionCss)
                                .AddClass("active", curItem == item)
                                .Build();
            //builder.Div(css, attr => builder.Fragment(item.ChildContent));
        }
    }

    private void OnItemClick(MenuItem item)
    {
        curItem = item;
        OnChanged?.Invoke(item);
    }
}