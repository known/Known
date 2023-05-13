namespace Known.Razor.Components;

public class DataList<TItem> : DataComponent<TItem>
{
    public DataList()
    {
        ContainerStyle = "list-view";
        ContentStyle = "list";
    }

    protected bool ShowStyleButton { get; set; } = false;
    protected string ListStyle { get; set; } = "squared";
    protected string ItemStyle { get; set; }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Data == null || Data.Count == 0)
        {
            BuildEmpty(builder);
        }
        else
        {
            BuildButtons(builder);
            BuildData(builder);
        }
    }

    private void BuildButtons(RenderTreeBuilder builder)
    {
        if (!ShowStyleButton)
            return;

        builder.Div("list-btns", attr =>
        {
            builder.Icon("fa fa-bars " + Active("lists"), attr =>
            {
                attr.Title("列表显示").OnClick(Callback(e => ListStyle = "lists"));
            });
            builder.Icon("fa fa-th-large " + Active("squared"), attr =>
            {
                attr.Title("宫格显示").OnClick(Callback(e => ListStyle = "squared"));
            });
        });
    }

    private void BuildData(RenderTreeBuilder builder)
    {
        if (ListStyle == "lists")
        {
            builder.Div("list-header", attr => BuildHead(builder));
        }
        foreach (TItem item in Data)
        {
            builder.Div($"list-item {ItemStyle}", attr => BuildItem(builder, item));
        }
    }

    protected virtual void BuildHead(RenderTreeBuilder builder) { }
    protected virtual void BuildItem(RenderTreeBuilder builder, TItem item) { }

    private string Active(string style) => ListStyle == style ? "active" : "";
}