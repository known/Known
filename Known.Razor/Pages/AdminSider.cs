namespace Known.Razor.Pages;

class AdminSider : BaseComponent
{
    private MenuItem curPage;
    private string IsActive(MenuItem menu) => curPage?.Id == menu.Id ? "active" : "";

    [Parameter] public MenuItem CurMenu { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Aside(attr =>
        {
            builder.Img(attr => attr.Src("img/logo.png"));
            BuildNavTree(builder);
        });
    }

    private void BuildNavTree(RenderTreeBuilder builder)
    {
        if (Menus == null || Menus.Count == 0)
            return;

        builder.Article(attr =>
        {
            foreach (var item in Menus)
            {
                builder.Details(attr =>
                {
                    BuildNavTitle(builder, item);
                    BuildNavChild(builder, item);
                });
            }
        });
    }

    private void BuildNavTitle(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Summary(attr =>
        {
            attr.OnClick(Callback(() => OnNavTitleClick(item)));
            builder.Icon(item.Icon, attr => attr.Title(item.Name));
            builder.Span(item.Name);
        });
    }

    private void BuildNavChild(RenderTreeBuilder builder, MenuItem item)
    {
        if (!item.Children.Any())
            return;

        builder.Ul(attr =>
        {
            foreach (var sub in item.Children)
            {
                var active = IsActive(sub);
                builder.Li(active, attr =>
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