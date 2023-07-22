namespace Known.Razor.Components;

class PageTabs : BaseComponent
{
    private readonly List<MenuItem> menus = new();
    private MenuItem curPage;
    private bool isClickClose;
    private string Active(string item) => curPage?.Id == item ? "active" : "";

    internal void ShowTab(MenuItem item)
    {
        if (!menus.Exists(m => m.Id == item.Id))
            menus.Add(item);

        OnItemClick(item);
    }

    protected override void OnInitialized()
    {
        if (menus.Count == 0)
        {
            menus.Add(KRConfig.Home);
            curPage = menus[0];
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("tabs top kui-tabs", attr =>
        {
            BuildTabHead(builder);
            BuildTabBody(builder);
        });
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitAdminTab();

        return base.OnAfterRenderAsync(firstRender);
    }

    private void BuildTabHead(RenderTreeBuilder builder)
    {
        builder.Div("kui-tab", attr =>
        {
            builder.Icon("btn-left fa fa-chevron-left");
            builder.Div("tab-wrapper", attr => BuildTab(builder));
            builder.Icon("btn-right fa fa-chevron-right");
            builder.Dropdown(new List<MenuItem>
            {
                new MenuItem("关闭当前", "fa fa-close", CloseCurrent),
                new MenuItem("关闭全部", "fa fa-close", CloseAll),
                new MenuItem("关闭其他", "fa fa-close", CloseOther)
            });
        });
    }

    private void BuildTab(RenderTreeBuilder builder)
    {
        builder.Ul("tab", attr =>
        {
            foreach (var item in menus)
            {
                var active = Active(item.Id);
                builder.Li(active, attr =>
                {
                    attr.Id($"th-{item.Id}").OnClick(Callback(() => OnItemClick(item)));
                    builder.IconName(item.Icon, item.Name);
                    if (item.Id != "Home")
                        builder.Icon("close fa fa-close", "", Callback(() => OnItemClose(item)));
                });
            }
        });
    }

    private void BuildTabBody(RenderTreeBuilder builder)
    {
        foreach (var item in menus)
        {
            var active = Active(item.Id);
            var css = CssBuilder.Default("tab-body").AddClass(active).Build();
            builder.Div(css, attr =>
            {
                attr.Id($"tb-{item.Id}").AddRandomColor("border-top-color");
                builder.DynamicComponent(item.ComType, item.ComParameters);
                if (item.Id != "Home")
                    builder.Component<DialogContainer>().Id(item.Id).Build();
            });
        }
    }

    private void OnItemClick(MenuItem menu)
    {
        if (isClickClose)
        {
            isClickClose = false;
            return;
        }

        curPage = menu;
        UI.PageId = curPage.Id;
        StateChanged();
    }

    private void OnItemClose(MenuItem item)
    {
        isClickClose = true;
        CloseTab(item);
    }

    private void CloseTab(MenuItem item)
    {
        if (curPage.Id == item.Id)
        {
            var index = menus.IndexOf(item);
            curPage = menus[index - 1];
            UI.PageId = curPage.Id;
        }
        UI.RemoveDialig(item.Id);
        menus.Remove(item);
        StateChanged();
    }

    private void CloseCurrent() => CloseTab(curPage);

    private void CloseAll()
    {
        UI.ClearDialog();
        menus.RemoveAll(m => m != KRConfig.Home);
        curPage = KRConfig.Home;
        StateChanged();
    }

    private void CloseOther()
    {
        var items = menus.Where(m => m != KRConfig.Home && m.Id != curPage.Id).ToList();
        foreach (var item in items)
        {
            UI.RemoveDialig(item.Id);
        }
        menus.RemoveAll(m => m != KRConfig.Home && m.Id != curPage.Id);
        StateChanged();
    }
}