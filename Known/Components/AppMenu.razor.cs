namespace Known.Components;

/// <summary>
/// 移动端菜单组件类。
/// </summary>
public partial class AppMenu
{
    private List<MenuInfo> MenuItems = [];
    private string Width => $"width:{100 / Count}%";

    /// <summary>
    /// 取得或设置每行菜单数量大小，默认每行2个。
    /// </summary>
    [Parameter] public int Count { get; set; } = 2;

    /// <summary>
    /// 取得或设置菜单项目列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置是否显示功能待加菜单。
    /// </summary>
    [Parameter] public bool ShowPending { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        MenuItems = Items?.Where(m => m.Target == "Menu").OrderBy(m => m.Sort).ToList();
    }

    private void OnItemClick(MenuInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.Url))
            return;

        Context.NavigateTo(item);
    }
}