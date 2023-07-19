namespace Known.Razor.Components;

public class Dropdown : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("dropdown", attr =>
        {
            if (!string.IsNullOrWhiteSpace(Text))
                builder.Span(Style, attr => builder.Text(Text));

            builder.Icon("fa fa-caret-down");
            builder.Ul("child box", attr => BuildItems(builder));
        });
    }

    private void BuildItems(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Count == 0)
            return;

        foreach (var item in Items)
        {
            builder.Li("item", attr =>
            {
                attr.OnClick(Callback(item.Action));
                builder.IconName(item.Icon, item.Name);
            });
        }
    }
}