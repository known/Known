namespace Known.Internals;

class KTopNavbar : BaseComponent
{
    private const string Key = "TopNavbar";
    private List<string> types = [];
    private string dragging;
    private bool isAdd;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        types = await GetTypesAsync();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        foreach (var item in types)
        {
            var type = GetNavItemType(item);
            if (type != null)
                builder.Li(() => builder.Component(type));
        }

        if (CurrentUser?.IsSystemAdmin() == true)
        {
            builder.Li(() =>
            {
                builder.Dropdown(new DropdownModel
                {
                    Tooltip = Language["Custom"],
                    TriggerType = "Click",
                    Icon = "edit",
                    Overlay = BuildOverlay
                });
            });
        }
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Overlay("kui-nav-setting", () =>
        {
            builder.Div("title", Language["Custom"]);
            foreach (var item in types)
            {
                builder.Div().Class("item").Draggable()
                       .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                       .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Child(() => BuildSettingItem(builder, item));
            }
            BuildAddItem(builder);
        });
    }

    private void BuildSettingItem(RenderTreeBuilder builder, string item)
    {
        builder.Div("span", () =>
        {
            builder.Icon("pause");
            builder.Span(item);
        });
        builder.Icon("delete", this.Callback<MouseEventArgs>(async e =>
        {
            types.Remove(item);
            await SaveConfigAsync();
        }));
    }

    private void BuildAddItem(RenderTreeBuilder builder)
    {
        if (isAdd)
        {
            builder.Div("item", () =>
            {
                builder.Select(new InputModel<string>
                {
                    Codes = GetNavItems(),
                    ValueChanged = this.Callback<string>(async v =>
                    {
                        types.Add(v);
                        isAdd = false;
                        await SaveConfigAsync();
                    })
                });
                builder.Icon("close", this.Callback<MouseEventArgs>(e => isAdd = false));
            });
        }
        else
        {
            builder.Icon("plus", this.Callback<MouseEventArgs>(e => isAdd = true));
        }
    }

    private async Task OnDropAsync(DragEventArgs e, string info)
    {
        if (info != null && dragging != null)
        {
            int index = types.IndexOf(info);
            types.Remove(dragging);
            types.Insert(index, dragging);
            dragging = null;
            await SaveConfigAsync();
            await StateChangedAsync();
        }
    }

    private void OnDragStart(DragEventArgs e, string info)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        dragging = info;
    }

    private List<CodeInfo> GetNavItems()
    {
        var navPulgins = Config.Plugins.Where(p => p.IsNavComponent).ToList();
        var items = new List<CodeInfo>();
        foreach (var item in navPulgins)
        {
            var name = item.Type.Name;
            if (types.Contains(name))
                continue;
            if (name == "NavFontSize" && !Config.App.IsSize)
                continue;
            if (name == nameof(NavLanguage) && !Config.App.IsLanguage)
                continue;
            if (name == nameof(NavTheme) && !Config.App.IsTheme)
                continue;

            items.Add(new CodeInfo(name, name));
        }
        return items;
    }

    private async Task<List<string>> GetTypesAsync()
    {
        var json = await Admin.GetConfigAsync(Key);
        var items = Utils.FromJson<List<string>>(json);
        if (items == null)
        {
            items = ["NavHome", "NavFullScreen"];
            if (Config.App.IsSize)
                items.Add("NavFontSize");
            if (Config.App.IsLanguage)
                items.Add(nameof(NavLanguage));
            items.Add(nameof(NavUser));
            if (Config.App.IsTheme)
                items.Add(nameof(NavTheme));
        }
        return items;
    }

    private static Type GetNavItemType(string item)
    {
        var plugin = Config.Plugins.FirstOrDefault(p => p.IsNavComponent && p.Type.Name == item);
        return plugin?.Type;
    }

    private Task SaveConfigAsync()
    {
        return Admin.SaveConfigAsync(new ConfigInfo { Key = Key, Value = types });
    }
}