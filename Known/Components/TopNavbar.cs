namespace Known.Components;

/// <summary>
/// 全局顶部导航工具条组件类。
/// </summary>
public class TopNavbar : BaseComponent
{
    private const string Key = "TopNavbar";
    private ISystemService Service;
    private List<string> types = [];
    private string dragging;
    private bool isAdd;

    /// <summary>
    /// 取得或设置按钮点击事件委托。
    /// </summary>
    [Parameter] public Action<string> OnMenuClick { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
        types = await GetTypesAsync();
    }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Ul("kui-nav", () =>
        {
            builder.Cascading(this, b =>
            {
                foreach (var item in types)
                {
                    if (!Config.NavItemTypes.TryGetValue(item, out var type))
                        continue;

                    b.Li().Children(() => b.Component(type)).Close();
                }
            });

            if (!Config.App.IsClient && CurrentUser?.IsSystemAdmin() == true)
            {
                builder.Li(() =>
                {
                    UI.BuildDropdown(builder, new DropdownModel
                    {
                        Tooltip = Language["Custom"],
                        TriggerType = "Click",
                        Icon = "edit",
                        Overlay = BuildOverlay
                    });
                });
            }
        });
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Div("kui-nav-setting", () =>
        {
            builder.Div("title", Language["Custom"]);
            foreach (var item in types)
            {
                builder.Div().Class("item").Draggable()
                     .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                     .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                     .Children(() => BuildSettingItem(builder, item))
                     .Close();
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
                UI.BuildSelect(builder, new InputModel<string>
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
        var items = new List<CodeInfo>();
        foreach (var item in Config.NavItemTypes.Keys)
        {
            if (types.Contains(item))
                continue;
            if (item == nameof(NavFontSize) && !Config.App.IsSize)
                continue;
            if (item == nameof(NavLanguage) && !Config.App.IsLanguage)
                continue;
            if (item == "AntTheme" && !Config.App.IsTheme)
                continue;

            items.Add(new CodeInfo(item, item));
        }
        return items;
    }

    private async Task<List<string>> GetTypesAsync()
    {
        var json = await Service.GetConfigAsync(Key);
        var items = Utils.FromJson<List<string>>(json);
        if (items == null)
        {
            items = ["NavHome", "NavFullScreen"];
            if (Config.App.IsSize)
                items.Add(nameof(NavFontSize));
            if (Config.App.IsLanguage)
                items.Add(nameof(NavLanguage));
            items.Add("NavUser");
            if (Config.App.IsTheme)
                items.Add("AntTheme");
            items.Add("NavSetting");
        }
        return items;
    }

    private Task SaveConfigAsync() => Service.SaveConfigAsync(new ConfigInfo { Key = Key, Value = types });
}