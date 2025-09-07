namespace Known.Plugins;

class PluginPage : BaseComponent, IAutoPage
{
    private ReloadContainer container = null;
    private List<ActionInfo> items = [];

    [Parameter] public string PageId { get; set; }
    [Parameter] public MenuInfo Menu { get; set; }
    [Parameter] public AutoPage Page { get; set; }

    internal Task<Result> SaveParameterAsync(string pluginId, object parameter)
    {
        var plugin = Menu.Plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin == null)
            return Result.ErrorAsync("插件不存在！");

        plugin.Setting = Utils.ToJson(parameter);
        return Platform.SaveMenuAsync(Menu);
    }

    internal async Task RemovePluginAsync(PluginInfo plugin)
    {
        Menu.Plugins.Remove(plugin);
        await Platform.SaveMenuAsync(Menu);
        await StateChangedAsync();
    }

    public Task InitializeAsync()
    {
        container?.Reload();
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
            builder.Component<ReloadContainer>()
                   .Set(c => c.ChildContent, BuildPageContent)
                   .Build(value => container = value);
        }

        if (Context.IsEditMode)
        {
            BuildLayout(builder);
            BuildAction(builder);
        }
    }

    private void BuildPageContent(RenderTreeBuilder builder)
    {
        builder.Cascading(this, b =>
        {
            if (Menu.Layout == null || Menu.Layout.Type == nameof(PageType.None))
            {
                Menu.Plugins?.ForEach(b.BuildPlugin);
            }
            else
            {
                var className = Menu.Layout.Type == nameof(PageType.Column)
                              ? $"kui-row-{Menu.Layout.Spans}"
                              : Menu.Layout.Custom;
                b.Div(className, () => Menu.Plugins.ForEach(b.BuildPlugin));
            }
        });
    }

    private void BuildLayout(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Class = "kui-edit",
            Icon = "layout",
            TriggerType = "Click",
            Text = "页面布局",
            Overlay = BuildLayoutOverlay
        };
        builder.Dropdown(model);
    }

    private void BuildLayoutOverlay(RenderTreeBuilder builder)
    {
        var data = Menu.Layout ?? new LayoutInfo();
        var form = new FormModel<LayoutInfo>(this)
        {
            SmallLabel = true,
            Data = data,
            OnFieldChanged = async v =>
            {
                Menu.Layout = data;
                await Platform.SaveMenuAsync(Menu);
                await StateChangedAsync();
            }
        };
        form.AddRow().AddColumn(c => c.Type);
        form.AddRow().AddColumn(c => c.Spans, c => c.ReadOnly = data.Type != nameof(PageType.Column));
        form.AddRow().AddColumn(c => c.Custom, c => c.ReadOnly = data.Type != nameof(PageType.Custom));
        builder.Overlay("kui-plugin-layout", () =>
        {
            builder.FormTitle("页面布局");
            builder.Form(form);
        });
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
        var plugins = PluginConfig.PagePlugins;
        var infos = new List<ActionInfo>();
        var types = Cache.GetCodes(nameof(PagePluginType));
        foreach (var item in types)
        {
            var type = Utils.ConvertTo<PagePluginType>(item.Code);
            var info = new ActionInfo { Id = item.Code, Name = item.Name, Icon = GetPluginIcon(type) };
            var itemMenus = plugins.Where(p => p.Attribute.Category == info.Id).OrderBy(p => p.Sort).ToList();
            var parents = itemMenus.Where(p => !string.IsNullOrWhiteSpace(p.Attribute.Parent))
                                   .Select(p => p.Attribute.Parent).Distinct().ToList();
            if (parents.Count > 0)
            {
                foreach (var parent in parents)
                {
                    var infoParent = new ActionInfo { Id = parent, Name = parent, Icon = "folder" };
                    var menus = itemMenus.Where(p => p.Attribute.Parent == parent).OrderBy(p => p.Sort).ToList();
                    foreach (var menu in menus)
                    {
                        infoParent.Children.Add(menu.ToAction());
                    }
                    info.Children.Add(infoParent);
                }
            }
            else
            {
                foreach (var menu in itemMenus)
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
            UI.Error(Language.TipMenuNotExists);
            return;
        }

        // 向当前页面添加插件实例
        Menu.Plugins ??= [];
        Menu.Plugins.Add(new PluginInfo
        {
            Id = Utils.GetNextId(),
            Type = info.Id
        });
        await Platform.SaveMenuAsync(Menu);
        await StateChangedAsync();
    }
}