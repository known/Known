namespace Known.Components;

/// <summary>
/// 操作按钮组件类。
/// </summary>
public partial class KAction
{
    private string Icon => ShowIcon? Item.Icon : string.Empty;

    /// <summary>
    /// 取得或设置操作按钮信息。
    /// </summary>
    [Parameter] public ActionInfo Item { get; set; }

    /// <summary>
    /// 取得或设置是否显示图标。
    /// </summary>
    [Parameter] public bool ShowIcon { get; set; }

    /// <summary>
    /// 取得或设置下拉框项目单击事件委托方法。
    /// </summary>
    [Parameter] public Action<ActionInfo> OnItemClick { get; set; }

    private DropdownModel GetDropdownModel()
    {
        return new DropdownModel
        {
            ChildContent = b => b.ButtonMore(Item, ShowIcon),
            Items = Item.Children,
            OnItemClick = e =>
            {
                OnItemClick?.Invoke(e);
                return Task.CompletedTask;
            }
        };
    }

    private void OnClick()
    {
        OnItemClick?.Invoke(Item);
    }
}