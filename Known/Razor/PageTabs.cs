namespace Known.Razor;

class PageTabs : BaseComponent
{
    private readonly List<KMenuItem> menus = new();
    private readonly ConcurrentDictionary<string, KDialogContainer> dialogs = new();
    private KMenuItem curPage;
    private string Active(string item) => curPage?.Id == item ? "active" : "";

    public PageTabs()
    {
        Id = "PageTabs";
    }

    internal void ShowTab(KMenuItem item)
    {
        if (!menus.Exists(m => m.Id == item.Id))
            menus.Add(item);

        curPage = item;
        StateChanged();
    }

    protected override void OnInitialized()
    {
        if (menus.Count == 0)
        {
            menus.Add(Config.Home);
            curPage = menus[0];
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-tabs", attr =>
        {
            builder.Cascading(this, b =>
            {
                BuildTabHead(b);
                BuildTabBody(b);
            });
        });
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitAdminTab();

        SetCurrentDialog();
        UI.InitPage(curPage?.Id);
        return base.OnAfterRenderAsync(firstRender);
    }

    private void BuildTabHead(RenderTreeBuilder builder)
    {
        builder.Div("kui-tab", attr =>
        {
            builder.Icon("btn-left fa fa-chevron-left");
            builder.Div("tab-wrapper", attr => BuildTab(builder));
            builder.Icon("btn-right fa fa-chevron-right");
            builder.Dropdown(new List<KMenuItem>
            {
                new KMenuItem("关闭当前", "fa fa-close", CloseCurrent),
                new KMenuItem("关闭全部", "fa fa-close", CloseAll),
                new KMenuItem("关闭其他", "fa fa-close", CloseOther)
            });
        });
    }

    private void BuildTab(RenderTreeBuilder builder)
    {
        builder.Ul("tab", attr =>
        {
            attr.Id("tabAdmin");
            foreach (var item in menus)
            {
                var active = Active(item.Id);
                builder.Li(active, attr =>
                {
                    attr.Id($"th-{item.Id}").OnClick(Callback(() => ShowTab(item)));
                    builder.IconName(item.Icon, item.Name);
                    if (item.Id != "Home")
                        builder.Icon("close fa fa-close", attr => attr.OnClick(Callback(() => CloseTab(item)), true));
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
                {
                    builder.Component<KDialogContainer>().Id(item.PageId).Build(async value =>
                    {
                        if (dialogs.ContainsKey(item.PageId))
                        {
                            await dialogs[item.PageId].DisposeAsync();
                        }
                        dialogs[item.PageId] = value;
                    });
                }
            });
        }
    }

    internal void CloseCurrent() => CloseTab(curPage);

    private async void CloseAll()
    {
        foreach (var item in dialogs)
        {
            await item.Value.DisposeAsync();
        }
        dialogs.Clear();
        menus.RemoveAll(m => m != Config.Home);
        curPage = Config.Home;
        UI.CurDialog = null;
        StateChanged();
    }

    private void CloseOther()
    {
        var items = menus.Where(m => m != Config.Home && m.Id != curPage.Id).ToList();
        foreach (var item in items)
        {
            RemoveDialig(item);
        }
        menus.RemoveAll(m => m != Config.Home && m.Id != curPage.Id);
        StateChanged();
    }

    private void CloseTab(KMenuItem item)
    {
        if (curPage.Id == item.Id)
        {
            var index = menus.IndexOf(item);
            curPage = menus[index - 1];
        }
        RemoveDialig(item);
        menus.Remove(item);
        StateChanged();
    }

    private void SetCurrentDialog()
    {
        dialogs.TryGetValue(curPage?.PageId, out KDialogContainer dialog);
        UI.CurDialog = dialog;
    }

    private async void RemoveDialig(KMenuItem item)
    {
        dialogs.TryRemove(item.PageId, out KDialogContainer dialog);
        await dialog.DisposeAsync();
    }
}