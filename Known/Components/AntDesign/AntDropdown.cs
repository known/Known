using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant下拉菜单组件类。
/// </summary>
public class AntDropdown : Dropdown
{
    [Inject] private IServiceScopeFactory Factory { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置下拉框设置模型。
    /// </summary>
    [Parameter] public DropdownModel Model { get; set; }

    /// <summary>
    /// 取得或设置下拉框的选中值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置下拉框的选中值改变事件。
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await OnInitializeAsync();
        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Class = Model?.Class;

        if (Model?.ChildContent != null)
            ChildContent = Model.ChildContent;
        else if (!string.IsNullOrWhiteSpace(Model?.Icon))
            ChildContent = BuildIcon;
        else if (!string.IsNullOrWhiteSpace(Model?.Text))
            ChildContent = BuildText;
        else if (!string.IsNullOrWhiteSpace(Model?.TextIcon))
            ChildContent = BuildTextIcon;
        else if (!string.IsNullOrWhiteSpace(Model?.TextButton))
            ChildContent = BuildTextButton;

        if (!string.IsNullOrWhiteSpace(Model?.TriggerType))
            Trigger = GetTriggers(Model?.TriggerType);

        if (Model?.Overlay != null)
            Overlay = Model?.Overlay;
        else if (Model?.Items != null && Model?.Items.Count > 0)
            Overlay = BuildOverlay;
    }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// 选中值改变事件。
    /// </summary>
    /// <param name="value">选中值。</param>
    protected virtual void OnValueChanged(string value)
    {
        Value = value;
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(Value);
    }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    public Task<T> CreateServiceAsync<T>() where T : IService => Factory.CreateAsync<T>(Context);

    private void BuildIcon(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Model?.Tooltip))
        {
            builder.Icon(Model?.Icon);
        }
        else
        {
            builder.Component<Tooltip>()
                   .Set(c => c.Title, Model?.Tooltip)
                   .Set(c => c.ChildContent, b => b.Icon(Model?.Icon))
                   .Build();
        }
        if (!string.IsNullOrWhiteSpace(Model?.Text))
            builder.Span(Model?.Text);
    }

    private void BuildText(RenderTreeBuilder builder)
    {
        builder.Element("a").Class("ant-dropdown-link").PreventDefault().Child(() =>
        {
            builder.Markup(Model?.Text);
            builder.Icon("down");
        });
    }

    private void BuildTextIcon(RenderTreeBuilder builder)
    {
        builder.Span().Role("img").Child(Model?.TextIcon);
    }

    private void BuildTextButton(RenderTreeBuilder builder)
    {
        builder.Component<Button>()
               .Set(c => c.ChildContent, b =>
               {
                   b.Markup(Model?.TextButton);
                   b.Icon("down");
               })
               .Build();
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Component<Menu>().Set(c => c.ChildContent, b => BuildMenu(b, Model?.Items)).Build();
    }

    private void BuildMenu(RenderTreeBuilder builder, List<ActionInfo> items)
    {
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            BuildMenu(builder, item);
        }
    }

    private void BuildMenu(RenderTreeBuilder builder, ActionInfo item)
    {
        if (item.Children != null && item.Children.Count > 0)
        {
            builder.Component<SubMenu>()
                   .Set(c => c.Key, item.Id)
                   .Set(c => c.Disabled, !item.Enabled)
                   .Set(c => c.TitleTemplate, b => b.IconName(item.Icon, item.Name))
                   .Set(c => c.ChildContent, b => BuildMenu(b, item.Children))
                   .Build();
        }
        else
        {
            BuildMenuItem(builder, item);
        }
    }

    private void BuildMenuItem(RenderTreeBuilder builder, ActionInfo item)
    {
        builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.Disabled, !item.Enabled)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e =>
               {
                   if (Model?.OnItemClick != null)
                       Model?.OnItemClick?.Invoke(item);
                   else if (item.OnClick.HasDelegate)
                       item.OnClick.InvokeAsync();
               }))
               .Set(c => c.ChildContent, b => b.IconName(item.Icon, item.Name))
               .Build();
    }

    private static Trigger[] GetTriggers(string triggerType)
    {
        if (triggerType == "Click")
            return [AntDesign.Trigger.Click];
        else if (triggerType == "ContextMenu")
            return [AntDesign.Trigger.ContextMenu];
        else if (triggerType == "Hover")
            return [AntDesign.Trigger.Hover];
        else if (triggerType == "Focus")
            return [AntDesign.Trigger.Focus];

        return [AntDesign.Trigger.None];
    }
}

/// <summary>
/// 扩展Ant表格选择框组件类。
/// </summary>
/// <typeparam name="TItem">数据项类型。</typeparam>
public class AntDropdownTable<TItem> : AntDropdown, IBaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得表格模型。
    /// </summary>
    protected TableModel<TItem> Table { get; private set; }

    /// <summary>
    /// 取得或设置输入组件文本占位符。
    /// </summary>
    public string Placeholder { get; set; } = Language.PleaseSelectInput;

    /// <summary>
    /// 取得组件显示取值委托。
    /// </summary>
    protected virtual Func<TItem, string> OnValue { get; }

    /// <summary>
    /// 取得或设置注入的UI服务实例。
    /// </summary>
    [Inject] public UIService UI { get; set; }

    /// <summary>
    /// 取得或设置选中行改变事件委托。
    /// </summary>
    [Parameter] public EventCallback<TItem> OnChange { get; set; }

    /// <summary>
    /// 异步刷新组件。
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync() => Table.RefreshAsync();

    /// <summary>
    /// 通知组件状态改变，重新呈现组件。
    /// </summary>
    public void StateChanged() => StateHasChanged();

    /// <summary>
    /// 异步通知组件状态改变，重新呈现组件。
    /// </summary>
    /// <returns></returns>
    public Task StateChangedAsync() => InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        Table = new TableModel<TItem>(this);
        Table.FixedHeight = "200px";
        Table.AdvSearch = false;
        Table.AutoHeight = false;
        //Table.IsScroll = false;
        Table.ShowSetting = false;
        Table.ShowPager = true;
        Table.OnRowClick = OnRowClick;
        Table.OnAction = (info, item) => Context.OnAction(this, info, [item]);
        Table.Toolbar.OnItemClick = info => Context.OnAction(this, info, null);

        Model = new DropdownModel
        {
            TriggerType = "Click",
            ChildContent = BuildContent,
            Overlay = BuildOverlay
        };
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Component<AntInput>()
               .Set(c => c.AllowClear, true)
               .Set(c => c.ReadOnly, true)
               .Set(c => c.Value, Value)
               .Set(c => c.ValueChanged, ValueChanged)
               .Set(c => c.Placeholder, Placeholder)
               .Set(c => c.Disabled, AntForm?.IsView == true)
               .Set(c => c.OnClear, this.Callback(OnClear))
               .Build();
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        var width = Table.Columns.Sum(c => c.Width ?? 100) + 60;
        if (width > 800) width = 800;
        if (width < 500) width = 500;
        var className = CssBuilder.Default("kui-card overlay").AddClass(Class).BuildClass();
        var style = CssBuilder.Default().AddStyle(Style).Add("width", $"{width}px").BuildStyle();
        builder.Div().Class(className).Style(style).Child(() => builder.FormTable(Table));
    }

    private Task OnRowClick(TItem item)
    {
        OnValueChanged(OnValue?.Invoke(item));
        if (OnChange.HasDelegate)
            OnChange.InvokeAsync(item);
        Close();
        return Task.CompletedTask;
    }

    private Task OnClear()
    {
        OnValueChanged("");
        if (OnChange.HasDelegate)
            OnChange.InvokeAsync(null);
        return Task.CompletedTask;
    }
}

/// <summary>
/// 扩展Ant树选择框组件类。
/// </summary>
public class AntDropdownTree : AntDropdown
{
    private TreeModel model;

    /// <summary>
    /// 取得或设置输入组件文本占位符。
    /// </summary>
    public string Placeholder { get; set; } = Language.PleaseSelectInput;

    /// <summary>
    /// 取得或设置未转换树节点的数据列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置选中行改变事件委托。
    /// </summary>
    [Parameter] public EventCallback<MenuInfo> OnChange { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        model = new TreeModel();
        model.SelectedKeys = [Value];
        model.OnNodeClick = n =>
        {
            OnValueChanged(n.Name ?? n.Code);
            if (OnChange.HasDelegate)
                OnChange.InvokeAsync(n);
            Close();
            return Task.CompletedTask;
        };
        model.Data = Items.ToMenuItems();

        Model = new DropdownModel
        {
            TriggerType = "Click",
            ChildContent = BuildContent,
            Overlay = BuildOverlay
        };
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Component<AntInput>()
               .Set(c => c.AllowClear, true)
               .Set(c => c.ReadOnly, true)
               .Set(c => c.Value, Value)
               .Set(c => c.ValueChanged, ValueChanged)
               .Set(c => c.Placeholder, Placeholder)
               .Set(c => c.Disabled, AntForm?.IsView == true)
               .Set(c => c.OnClear, this.Callback(OnClear))
               .Build();
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-card overlay").AddClass(Class).BuildClass();
        var style = CssBuilder.Default().Add("min-width", "200px").AddStyle(Style).BuildStyle();
        builder.Div().Class(className).Style(style).Child(() => builder.Tree(model));
    }

    private Task OnClear()
    {
        OnValueChanged("");
        if (OnChange.HasDelegate)
            OnChange.InvokeAsync(null);
        return Task.CompletedTask;
    }
}