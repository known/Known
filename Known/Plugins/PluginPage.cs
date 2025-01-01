namespace Known.Plugins;

class PluginPage : BaseComponent, IAutoPage
{
    private List<ActionInfo> items = [];

    [Parameter] public MenuInfo Menu { get; set; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        items = GetActionItems();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Menu != null && Menu.Plugins != null && Menu.Plugins.Count > 0)
        {
            builder.Cascading(this, b =>
            {
                foreach (var item in Menu.Plugins)
                {
                    b.BuildPlugin(item);
                }
            });
        }

        if (UIConfig.IsEditMode)
            BuildAction(builder);
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Class = "kui-edit",
            Icon = "plus",
            Items = items,
            TriggerType = "Click",
            Text = "添加区块",
            OnItemClick = OnItemClickAsync
        };
        builder.Dropdown(model);
    }

    private static List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.IsPage).ToList();
        var infos = new List<ActionInfo>();
        var types = Cache.GetCodes(nameof(PagePluginType));
        foreach (var item in types)
        {
            var type = Utils.ConvertTo<PagePluginType>(item.Code);
            var info = new ActionInfo { Id = item.Code, Name = item.Name, Icon = GetPluginIcon(type) };
            var menus = plugins.Where(p => p.Attribute.Category == info.Id).ToList();
            if (menus != null && menus.Count > 0)
            {
                foreach (var menu in menus)
                {
                    info.Children.Add(menu.ToAction());
                }
            }
            infos.Add(info);
        }
        return infos;
    }

    private static string GetPluginIcon(PagePluginType type)
    {
        return type switch
        {
            PagePluginType.Module => "appstore",
            PagePluginType.Table => "table",
            PagePluginType.Form => "form",
            PagePluginType.Detail => "profile",
            PagePluginType.List => "unordered-list",
            PagePluginType.Chart => "bar-chart",
            PagePluginType.Template => "block",
            PagePluginType.AI => "robot",
            PagePluginType.Other => "folder",
            _ => "",
        };
    }

    private async Task OnItemClickAsync(ActionInfo info)
    {
        if (Menu == null)
        {
            UI.Error("菜单不存在！");
            return;
        }

        // 向当前页面添加插件实例
        Menu.Plugins.Add(new PluginInfo
        {
            Id = Utils.GetNextId(),
            Type = info.Id
        });
        await Platform.SaveMenuAsync(Menu);
        await StateChangedAsync();
    }
}