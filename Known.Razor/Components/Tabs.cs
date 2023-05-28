namespace Known.Razor.Components;

public class Tabs : BaseComponent
{
    private string curItem;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Position { get; set; } = "top";
    [Parameter] public TabItem[] Items { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override Task OnInitializedAsync()
    {
        curItem = Items?.First().Title;
        return base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Length == 0)
            return;

        var css = CssBuilder.Default("tabs").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            BuildTabHead(builder);
            BuildTabBody(builder);
        });
    }

    private void BuildTabHead(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("tab").AddClass(Position).Build();
        builder.Ul(css, attr =>
        {
            foreach (var item in Items)
            {
                builder.Li(curItem == item.Title ? "active" : "", attr =>
                {
                    attr.OnClick(Callback(e => OnItemClick(item.Title)));
                    builder.IconName(item.Icon, item.Title);
                });
            }
        });
    }

    private void BuildTabBody(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var css = CssBuilder.Default("tab-body")
                                .AddClass(Position)
                                .AddClass("active", curItem == item.Title)
                                .Build();
            builder.Div(css, attr => builder.Fragment(item.ChildContent));
        }
    }

    private void OnItemClick(string item)
    {
        curItem = item;
        OnChanged?.Invoke(item);
    }
}

public class TabItem
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public RenderFragment ChildContent { get; set; }
}