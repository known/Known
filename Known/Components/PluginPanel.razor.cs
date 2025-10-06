namespace Known.Components;

/// <summary>
/// 插件面板组件类。
/// </summary>
public partial class PluginPanel
{
    private string ClassName => CssBuilder.Default("kui-plugin").AddClass(Class).BuildClass();
    private DropdownModel model;

    /// <summary>
    /// 取得或设置插件是否可以编辑，默认可编辑。
    /// </summary>
    [Parameter] public bool CanEdit { get; set; } = true;

    /// <summary>
    /// 取得或设置插件是否可以拖拽。
    /// </summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>
    /// 取得或设置插件配置下拉菜单项列表。
    /// </summary>
    [Parameter] public List<ActionInfo> Actions { get; set; }

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model = new DropdownModel
        {
            Icon = "menu",
            Items = Actions,
            TriggerType = "Click",
            OnItemClick = OnItemClickAsync
        };
    }

    private Task OnItemClickAsync(ActionInfo info)
    {
        if (!info.OnClick.HasDelegate)
            return Task.CompletedTask;

        return info.OnClick.InvokeAsync();
    }
}