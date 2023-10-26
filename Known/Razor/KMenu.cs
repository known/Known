namespace Known.Razor;

public class KMenu : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<KMenuItem> Items { get; set; }
    [Parameter] public Action<KMenuItem> OnClick { get; set; }
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

    private void BuildItem(RenderTreeBuilder builder, KMenuItem item)
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

    private void BuildItem(RenderTreeBuilder builder, AttributeBuilder attr, KMenuItem item)
    {
        attr.OnClick(Callback(e => OnClick?.Invoke(item)));

        if (OnlyIcon)
            BuildOnlyIconItem(builder, item);
        else if (TextIcon)
            BuildTextIconItem(builder, item);
        else
            BuildTextItem(builder, item);
    }

    private static void BuildOnlyIconItem(RenderTreeBuilder builder, KMenuItem item)
    {
        builder.Icon(item.Icon, attr =>
        {
            attr.Title(item.Name);
            BuildBadge(builder, item);
        });
    }

    private static void BuildTextIconItem(RenderTreeBuilder builder, KMenuItem item)
    {
        builder.Span("item", attr =>
        {
            builder.Icon(item.Icon, attr =>
            {
                attr.Title(item.Name).Style($"background-color:{item.Color}");
                BuildBadge(builder, item);
            });
            builder.Span("name", item.Name);
        });
    }

    private static void BuildTextItem(RenderTreeBuilder builder, KMenuItem item)
    {
        builder.Span("item", attr =>
        {
            builder.Text(item.Name);
            BuildBadge(builder, item);
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder, KMenuItem item)
    {
        if (item.Badge > 0)
        {
            builder.Span("badge-top", $"{item.Badge}");
        }
    }
}