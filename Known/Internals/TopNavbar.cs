namespace Known.Internals;

/// <summary>
/// 全局顶部导航工具条组件类。
/// </summary>
public class TopNavbar : BaseComponent
{
    private List<PluginInfo> items;
    private PluginInfo dragging;
    private NavAction action;

    private List<string> Values => items?.Select(i => i.Id).ToList();

    /// <summary>
    /// 取得或设置系统设置按钮点击事件委托。
    /// </summary>
    [Parameter] public Action OnSetting { get; set; }

    /// <summary>
    /// 取得或设置导航菜单点击事件委托。
    /// </summary>
    [Parameter] public Action<ActionInfo> OnActionClick { get; set; }

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
                if (Context.IsEditMode)
                    BuildNavAction(b);

                if (UIConfig.TopNavType != null)
                    b.DynamicComponent(UIConfig.TopNavType);
                else
                    BuildTopNavbar(b);

                if (CurrentUser?.IsSystemAdmin() == true)
                {
                    if (UIConfig.EnableEdit)
                    {
                        var className = Context.IsEditMode ? "edit" : "";
                        b.Li().Class(className).Child(() => b.Component<NavEditMode>().Build());
                    }
                    b.Li(() => b.Component<NavDevelopment>().Build());
                }
                b.Li(() => b.Component<NavSetting>().Build());
            });
        });
    }

    private void BuildNavAction(RenderTreeBuilder builder)
    {
        builder.Li().Class("kui-edit").Child(() =>
        {
            builder.Component<NavAction>()
                   .Set(c => c.Values, Values)
                   .Set(c => c.OnAdded, OnNavbarAddedAsync)
                   .Build(value => action = value);
        });
        builder.Li().Class("kui-delete").Draggable()
               .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, null)))
               .Child(() => builder.Icon("delete"));
    }

    private void BuildTopNavbar(RenderTreeBuilder builder)
    {
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            if (Context.IsEditMode)
            {
                builder.Li().Draggable()
                       .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                       .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Child(() => builder.BuildPlugin(item));
            }
            else
            {
                builder.Li(() => builder.BuildPlugin(item));
            }
        }
    }

    private async Task OnDropAsync(DragEventArgs e, PluginInfo item)
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

    private void OnDragStart(DragEventArgs e, PluginInfo item)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        dragging = item;
    }

    private async Task<Result> OnNavbarAddedAsync(PluginInfo info)
    {
        items.Add(info);
        await Platform.SaveTopNavsAsync(items);
        await StateChangedAsync();
        action?.SetValues(Values);
        return Result.Success(Language.Success(Language.New));
    }
}