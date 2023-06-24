namespace Known.Razor.Pages;

class AdminSider : BaseComponent
{
    private string themeColor;
    private string sideColor;

    [Parameter] public MenuItem CurMenu { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }

    protected override void OnInitialized()
    {
        themeColor = Setting.Info.ThemeColor;
        sideColor = Setting.Info.SideColor;
        PageAction.RefreshSideColor = () =>
        {
            themeColor = Setting.Info.ThemeColor;
            sideColor = Setting.Info.SideColor;
            StateChanged();
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-sider", attr =>
        {
            attr.Style($"background-color:{sideColor}");
            builder.Div("logo", attr =>
            {
                attr.Style($"background-color:{themeColor}");
                builder.Img(attr => attr.Src("img/logo.png"));
            });
            builder.Div("kui-scroll", attr => BuildMenuTree(builder));
        });
    }

    private void BuildMenuTree(RenderTreeBuilder builder)
    {
        builder.Component<Menu>()
               .Set(c => c.Style, "menu-tree")
               .Set(c => c.TextIcon, true)
               .Set(c => c.Items, Menus)
               .Set(c => c.OnClick, OnNavItemClick)
               .Build();
    }

    private void OnNavItemClick(MenuItem item) => Context.Navigate(item);
}