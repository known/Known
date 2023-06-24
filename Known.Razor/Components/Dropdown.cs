namespace Known.Razor.Components;

public class Dropdown : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("dropdown", attr =>
        {
            builder.Span(Style, attr =>
            {
                builder.Text(Title);
            });
            builder.Icon("fa fa-caret-down");
            builder.Ul("child box", attr =>
            {
                foreach (var item in Items)
                {
                    builder.Li("item", attr =>
                    {
                        attr.OnClick(Callback(item.Action));
                        builder.IconName(item.Icon, item.Name);
                    });
                }
            });
        });
    }
}