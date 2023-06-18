namespace Known.Razor.Components;

public class Dropdown : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public List<DropdownItem> Items { get; set; }

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
                        attr.OnClick(Callback(item.Click));
                        builder.IconName(item.Icon, item.Name);
                    });
                }
            });
        });
    }
}

public class DropdownItem
{
    public DropdownItem() { }
    internal DropdownItem(ButtonInfo info, Action click)
    {
        Icon = info.Icon;
        Name = info.Name;
        Click = click;
    }

    public string Icon { get; set; }
    public string Name { get; set; }
    public Action Click { get; set; }
}