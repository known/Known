namespace Known.Razor.Pages;

class PageTabs : BaseComponent
{
    private string Active(string item) => CurPage?.Id == item ? "active" : "";

    [Parameter] public MenuItem CurPage { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("tabs kui-tabs", attr =>
        {
            BuildTabHead(builder);
            BuildTabBody(builder);
        });
    }

    private void BuildTabHead(RenderTreeBuilder builder)
    {
        if (Menus == null || Menus.Count == 0)
            return;

        builder.Ul("tab top header", attr =>
        {
            foreach (var item in Menus)
            {
                var active = Active(item.Id);
                builder.Li(active, attr =>
                {
                    builder.Span(attr =>
                    {
                        attr.OnClick(Callback(() => OnItemClick(item)));
                        builder.IconName(item.Icon, item.Name);
                    });
                    if (item.Id != "Home")
                        builder.Icon("close fa fa-close", attr => attr.OnClick(Callback(() => OnItemClose(item))));
                });
            }
        });
    }

    private void BuildTabBody(RenderTreeBuilder builder)
    {
        if (Menus == null || Menus.Count == 0)
            return;

        foreach (var item in Menus)
        {
            var active = Active(item.Id);
            builder.Div($"tab-body top {active}", attr =>
            {
                builder.DynamicComponent(item.ComType, item.ComParameters);
                if (item.Id != "Home")
                    builder.Component<DialogContainer>().Id(item.Id).Build();
            });
        }
    }

    private void OnItemClick(MenuItem menu)
    {
        CurPage = menu;
        UI.PageId = CurPage.Id;
        StateChanged();
    }

    private void OnItemClose(MenuItem menu)
    {
        var curIndex = Menus.IndexOf(CurPage);
        var index = Menus.IndexOf(menu);
        Menus.Remove(menu);
        if (index >= curIndex && curIndex > 0)
        {
            CurPage = index >= Menus.Count ? Menus.Last() : Menus[index];
            UI.PageId = CurPage.Id;
        }
        StateChanged();
    }
}