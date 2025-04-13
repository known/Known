namespace Known.Components;

/// <summary>
/// 移动端标签组件类。
/// </summary>
public partial class AppTab
{
    private string current;

    /// <summary>
    /// 取得或设置标签菜单项目列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        current = Items?.FirstOrDefault()?.Id;
    }

    private void OnItemClick(MenuInfo item)
    {
        current = item.Id;
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