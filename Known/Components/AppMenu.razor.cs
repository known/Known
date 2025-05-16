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

    /// <summary>
    /// 取得或设置菜单按钮点击事件委托。
    /// </summary>
    [Parameter] public Action<MenuInfo> OnClick { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        MenuItems = Items?.Where(m => m.Target == "Menu").OrderBy(m => m.Sort).ToList();
    }

    private void OnItemClick(MenuInfo item)
    {
        if (OnClick != null)
            OnClick.Invoke(item);
        else
            Context.NavigateTo(item);
    }

    private bool CheckItem(MenuInfo item)
    {
        if (!item.Visible)
            return false;

        if (string.IsNullOrWhiteSpace(item.Role))
            return true;

        var roles = item.Role.Split(',');
        return CurrentUser.InRole(roles);
    }
}