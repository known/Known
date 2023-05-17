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

        builder.Div($"tabs {Style}", attr =>
        {
            BuildTabHead(builder);
            BuildTabBody(builder);
        });
    }

    private void BuildTabHead(RenderTreeBuilder builder)
    {
        builder.Ul($"tab {Position}", attr =>
        {
            foreach (var item in Items)
            {
                builder.Li(Active(item.Title), attr =>
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
            builder.Div($"tab-body {Position} {Active(item.Title)}", attr => builder.Fragment(item.ChildContent));
        }
    }

    private void OnItemClick(string item)
    {
        curItem = item;
        OnChanged?.Invoke(item);
    }

    private string Active(string item) => curItem == item ? "active" : "";
}

public class TabItem
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public RenderFragment ChildContent { get; set; }
}