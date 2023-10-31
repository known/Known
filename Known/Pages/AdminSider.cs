namespace Known.Pages;

class AdminSider : BaseComponent
{
    [Parameter] public KMenuItem CurMenu { get; set; }
    [Parameter] public List<KMenuItem> Menus { get; set; }

    [CascadingParameter] internal Admin Admin { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-scroll", attr => BuildMenuTree(builder));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Admin.TopMenu)
            UI.InitMenu();

        await base.OnAfterRenderAsync(firstRender);
    }

    private void BuildMenuTree(RenderTreeBuilder builder)
    {
        builder.Component<KMenu>()
               .Set(c => c.Style, "menu-tree")
               .Set(c => c.TextIcon, true)
               .Set(c => c.Items, Menus)
               .Set(c => c.OnClick, OnNavItemClick)
               .Build();
    }

    private void OnNavItemClick(KMenuItem item) => Context.Navigate(item);
}