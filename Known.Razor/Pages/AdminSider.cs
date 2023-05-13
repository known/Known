namespace Known.Razor.Pages;

class AdminSider : BaseComponent
{
    private MenuItem curPage;
    private string IsShow(MenuItem menu) => CurMenu?.Id == menu.Id ? "show" : "";
    private string IsActive(MenuItem menu) => curPage?.Id == menu.Id ? "active" : "";

    [Parameter] public MenuItem CurMenu { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-sider", attr =>
        {
            builder.Div("logo", attr => builder.Img(attr => attr.Src("img/logo.png")));
            builder.Div("kui-scroll", attr => BuildNavTree(builder));
        });
    }

    private void BuildNavTree(RenderTreeBuilder builder)
    {
        if (Menus == null || Menus.Count == 0)
            return;

        builder.Ul("nav-tree", attr =>
        {
            foreach (var item in Menus)
            {
                builder.Li("nav-item", attr =>
                {
                    BuildNavTreeTitle(builder, item);
                    BuildNavTreeChild(builder, item);
                });
            }
        });
    }

    private void BuildNavTreeTitle(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Div("nav-title", attr =>
        {
            attr.OnClick(Callback(() => OnNavTitleClick(item)));
            builder.Icon(item.Icon, attr => attr.Title(item.Name));
            builder.Span(item.Name);
            if (item.Children.Any())
                builder.Span("fa fa-chevron-down", "");
        });
    }

    private void BuildNavTreeChild(RenderTreeBuilder builder, MenuItem item)
    {
        if (!item.Children.Any())
            return;

        var isShow = IsShow(item);
        builder.Dl($"nav-child {isShow}", attr =>
        {
            foreach (var sub in item.Children)
            {
                var active = IsActive(sub);
                builder.Dd(active, attr =>
                {
                    attr.OnClick(Callback(() => OnNavItemClick(sub)));
                    builder.Icon(sub.Icon, attr => attr.Title(sub.Name));
                    builder.Span(sub.Name);
                });
            }
        });
    }

    private void OnNavTitleClick(MenuItem item)
    {
        CurMenu = item;
        if (!item.Children.Any())
        {
            Context.Navigate(item);
            return;
        }
        StateChanged();
    }

    private void OnNavItemClick(MenuItem item)
    {
        curPage = item;
        Context.Navigate(item);
    }
}