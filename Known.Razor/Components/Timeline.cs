namespace Known.Razor.Components;

public class Timeline : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<TimelineItem> Items { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("timeline").AddClass(Style).Build();
        builder.Ul(css, attr =>
        {
            if (Items != null && Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    BuildItem(builder, item);
                }
            }
        });
    }

    private static void BuildItem(RenderTreeBuilder builder, TimelineItem item)
    {
        builder.Li(item.Type.ToString().ToLower(), attr =>
        {
            builder.Div("item", attr =>
            {
                if (item.Template != null)
                {
                    item.Template.Invoke(builder);
                }
                else
                {
                    builder.Span("name", item.Title);
                    if (item.Time != null)
                        builder.Span("time", $"{item.Time:yyyy-MM-dd HH:mm:ss}");
                    if (!string.IsNullOrWhiteSpace(item.Description))
                        builder.Span("text", item.Description);
                }
            });
        });
    }
}

public class TimelineItem
{
    public StyleType Type { get; set; }
    public DateTime? Time { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Action<RenderTreeBuilder> Template { get; set; }
}