namespace Known.Razor.Components;

public class Menu : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }
    [Parameter] public Action<MenuItem> OnClick { get; set; }
    [Parameter] public bool OnlyIcon { get; set; }
    [Parameter] public bool TextIcon { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Count == 0)
            return;

        builder.Ul(Style, attr =>
        {
            foreach (var item in Items)
            {
                BuildItem(builder, item);
            }
        });
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitMenu();

        return base.OnAfterRenderAsync(firstRender);
    }

    private void BuildItem(RenderTreeBuilder builder, MenuItem item)
    {
        var css = CssBuilder.Default("").AddClass("child", item.Children.Any()).Build();
        builder.Li(css, attr =>
        {
            BuildItem(builder, attr, item);
            if (item.Children.Any())
            {
                builder.Ul(attr =>
                {
                    foreach (var sub in item.Children)
                    {
                        BuildItem(builder, sub);
                    }
                });
            }
        });
    }

    private void BuildItem(RenderTreeBuilder builder, AttributeBuilder attr, MenuItem item)
    {
        attr.OnClick(Callback(e => OnClick?.Invoke(item)));

        if (OnlyIcon)
            BuildOnlyIconItem(builder, item);
        else if (TextIcon)
            BuildTextIconItem(builder, item);
        else
            BuildTextItem(builder, item);
    }

    private static void BuildOnlyIconItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Icon(item.Icon, attr =>
        {
            attr.Title(item.Name);
            BuildBadge(builder, item);
        });
    }

    private static void BuildTextIconItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Span("item", attr =>
        {
            builder.Icon(item.Icon, attr =>
            {
                attr.Style($"background-color:{item.Color}");
                BuildBadge(builder, item);
            });
            builder.Span("name", item.Name);
        });
    }

    private static void BuildTextItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Span("item", attr =>
        {
            builder.Text(item.Name);
            BuildBadge(builder, item);
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder, MenuItem item)
    {
        if (item.Badge > 0)
        {
            builder.Span("badge-top", $"{item.Badge}");
        }
    }
}