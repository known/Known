namespace Known.Components;

/// <summary>
/// 移动端菜单组件类。
/// </summary>
public partial class AppMenu
{
    private double Width => 100 / Count;

    /// <summary>
    /// 取得或设置每行菜单数量大小，默认每行2个。
    /// </summary>
    [Parameter] public int Count { get; set; } = 2;

    /// <summary>
    /// 取得或设置菜单项目列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    private void OnItemClick(MenuInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.Url))
            return;

        Context.NavigateTo(item);
    }
}