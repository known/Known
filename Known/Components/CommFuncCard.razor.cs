namespace Known.Components;

/// <summary>
/// 常用功能卡片组件类，
/// </summary>
public partial class CommFuncCard
{
    /// <summary>
    /// 取得或设置常用功能菜单ID列表。
    /// </summary>
    [Parameter] public List<string> Menus { get; set; }

    /// <summary>
    /// 设置常用功能菜单ID列表。
    /// </summary>
    /// <param name="menus">菜单ID列表。</param>
    public void SetMenus(List<string> menus)
    {
        Menus = menus;
        StateChanged();
    }
}