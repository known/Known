namespace Known.Internals;

/// <summary>
/// 全局顶部导航工具条组件类。
/// </summary>
public class TopNavbar : BaseComponent
{
    private List<TopNavInfo> items;
    private TopNavInfo dragging;
    private NavAction action;

    private List<string> Values => items.Select(i => i.PluginId).ToList();

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
        items = await Platform.GetTopNavsAsync();
        items ??= [];
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
                if (UIConfig.IsEditMode)
                {
                    b.Li().Class("kui-edit").Child(() =>
                    {
                        b.Component<NavAction>()
                         .Set(c => c.Values, Values)
                         .Set(c => c.OnAdded, OnNavbarAddedAsync)
                         .Build(value => action = value);
                    });
                    b.Li().Class("kui-delete").Draggable()
                     .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, null)))
                     .Child(() => b.Icon("delete"));
                }

                if (UIConfig.TopNavType != null)
                    b.DynamicComponent(UIConfig.TopNavType);
                else
                    BuildTopNavbar(b);

                if (CurrentUser?.IsSystemAdmin() == true)
                {
                    if (UIConfig.EnableEdit)
                    {
                        var className = UIConfig.IsEditMode ? "edit" : "";
                        b.Li().Class(className).Child(() => b.Component<NavEditMode>().Build());
                    }
                    b.Li(() => b.Component<NavDevelopment>().Build());
                }
                b.Li(() => b.Component<NavSetting>().Build());
            });
        });
    }

    private void BuildTopNavbar(RenderTreeBuilder builder)
    {
        if (items.Count == 0)
            return;

        foreach (var item in items)
        {
            if (UIConfig.IsEditMode)
            {
                builder.Li().Draggable()
                       .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                       .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Child(() => BuildNavItem(builder, item));
            }
            else
            {
                builder.Li(() => BuildNavItem(builder, item));
            }
        }
    }

    private void BuildNavItem(RenderTreeBuilder builder, TopNavInfo item)
    {
        var plugin = Config.Plugins.FirstOrDefault(p => p.Id == item.PluginId);
        if (plugin == null)
            return;

        plugin.Parameters = item.Parameters;
        if (plugin.IsNavComponent)
            builder.DynamicComponent(plugin.Type);
        else
            builder.BuildPlugin(this, plugin);
    }

    private async Task OnDropAsync(DragEventArgs e, TopNavInfo item)
    {
        if (dragging == null)
            return;

        if (item != null)
        {
            var index = items.IndexOf(item);
            if (index >= 0)
            {
                items.Remove(dragging);
                items.Insert(index, dragging);
            }
        }
        else
        {
            items.Remove(dragging);
        }
        dragging = null;
        await Platform.SaveTopNavsAsync(items);
        await StateChangedAsync();
        action?.SetValues(Values);
    }

    private void OnDragStart(DragEventArgs e, TopNavInfo item)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        dragging = item;
    }

    private async Task<Result> OnNavbarAddedAsync(TopNavInfo info)
    {
        items.Add(info);
        await Platform.SaveTopNavsAsync(items);
        await StateChangedAsync();
        action?.SetValues(Values);
        return Result.Success(Language.Success(Language.New));
    }
}