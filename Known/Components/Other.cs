namespace Known.Components;

/// <summary>
/// 面板组件基类。
/// </summary>
public abstract class KPanelBase<TItem> : BaseComponent
{
    /// <summary>
    /// 取得或设置是否显示搜索。
    /// </summary>
    [Parameter] public bool ShowSearch { get; set; }

    /// <summary>
    /// 取得或设置是否显示列表添加按钮。
    /// </summary>
    [Parameter] public bool ShowAddButton { get; set; }

    /// <summary>
    /// 取得或设置列表添加按钮名称。
    /// </summary>
    [Parameter] public string AddButtonText { get; set; } = Language.AddData;

    /// <summary>
    /// 取得或设置添加数据按钮单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnAddClick { get; set; }

    /// <summary>
    /// 取得或设置列表项数据源。
    /// </summary>
    [Parameter] public List<TItem> DataSource { get; set; }

    /// <summary>
    /// 取得或设置列表项单击事件。
    /// </summary>
    [Parameter] public EventCallback<TItem> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }
}