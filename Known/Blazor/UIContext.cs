namespace Known.Blazor;

/// <summary>
/// UI上下文信息类。
/// </summary>
public partial class UIContext : Context
{
    internal bool IsEditTable => UIConfig.IsEditTable && IsEditMode;

    /// <summary>
    /// 取得或设置界面是否是编辑模式。
    /// </summary>
    public bool IsEditMode { get; set; }

    /// <summary>
    /// 取得或设置当前主题。
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 取得当前页面会话生命周期运行时间。
    /// </summary>
    public Dictionary<string, DateTime> RunTimes { get; } = [];

    /// <summary>
    /// 取得或设置当前用户设置用户系统设置信息对象。
    /// </summary>
    public UserSettingInfo UserSetting { get; set; } = new();

    /// <summary>
    /// 取得或设置当前用户模块表格设置信息列表。
    /// </summary>
    public Dictionary<string, List<TableSettingInfo>> UserTableSettings { get; set; } = [];

    /// <summary>
    /// 取得或设置当前用户权限菜单信息列表。
    /// </summary>
    public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 取得UI服务实例。
    /// </summary>
    public UIService UI { get; internal set; }

    /// <summary>
    /// 取得基础模板实例。
    /// </summary>
    public BaseLayout App { get; internal set; }

    /// <summary>
    /// 取得当前组件呈现模式。
    /// </summary>
    public IComponentRenderMode RenderMode
    {
        get
        {
            return Config.CurrentMode switch
            {
                RenderType.Auto => new InteractiveAutoRenderMode(false),
                _ => new InteractiveServerRenderMode(false),
            };
        }
    }

    /// <summary>
    /// 根据菜单ID获取菜单信息列表。
    /// </summary>
    /// <param name="menuIds">菜单ID列表。</param>
    /// <returns>菜单信息列表。</returns>
    public List<MenuInfo> GetMenus(List<string> menuIds)
    {
        if (menuIds == null || menuIds.Count == 0)
            return [];

        var menus = new List<MenuInfo>();
        foreach (var menuId in menuIds)
        {
            var menu = UserMenus?.FirstOrDefault(m => m.Name == menuId);
            if (menu != null)
                menus.Add(menu);
        }
        return menus;
    }

    /// <summary>
    /// 判断当前用户是否有按钮权限。
    /// </summary>
    /// <param name="buttonId">按钮ID。</param>
    /// <returns>是否有权限。</returns>
    public bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        if (user.IsAdmin())
            return true;

        return IsInMenu(Current?.Id, buttonId);
    }

    /// <summary>
    /// 用户退出。
    /// </summary>
    public void SignOut()
    {
        CurrentUser = null;
        UserMenus = null;
    }

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var param = menu.GetAutoPageParameter();
        if (param == null || param.Page == null)
            return false;

        var hasButton = false;
        if (param.Page.Tools != null && param.Page.Tools.Count > 0)
            hasButton = param.Page.Tools.Contains(buttonId);
        if (!hasButton && param.Page.Actions != null && param.Page.Actions.Count > 0)
            hasButton = param.Page.Actions.Contains(buttonId);
        return hasButton;
    }
}