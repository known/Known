namespace Known.Extensions;

static class CoreMenuExtension
{
    internal static async Task<List<MenuInfo>> GetUserMenusAsync(this Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

        var menus = await DataHelper.GetMenusAsync(db);
        // 如果是管理员，返回所有菜单
        if (user.IsAdmin())
            return menus;

        // 如果是角色用户，根据用户角色模块ID列表返回菜单
        var moduleIds = await CoreConfig.OnRoleModule?.Invoke(db, user.Id);
        if (moduleIds == null || moduleIds.Count == 0)
            return [];

        var userMenus = new List<MenuInfo>();
        foreach (var item in menus)
        {
            if (!item.Enabled)
                continue;

            if (!moduleIds.Contains(item.Id))
                continue;

            if (userMenus.Exists(m => m.Id == item.Id))
                continue;

            var menu = item.Clone();
            AddParentModule(menus, userMenus, menu);
            SetPluginPermission(menu, moduleIds);
            userMenus.Add(menu);
        }

        return userMenus;
    }

    private static void AddParentModule(List<MenuInfo> allMenus, List<MenuInfo> userMenus, MenuInfo info)
    {
        // 如果父模块不存在，则添加父模块
        if (!userMenus.Exists(m => m.Id == info.ParentId))
        {
            var parent = allMenus.FirstOrDefault(m => m.Id == info.ParentId);
            if (parent != null)
            {
                userMenus.Add(parent);
                AddParentModule(allMenus, userMenus, parent);
            }
        }
    }

    private static void SetPluginPermission(MenuInfo info, List<string> moduleIds)
    {
        var param = info?.Plugins?.GetPluginParameter<AutoPageInfo>();
        if (param == null)
            return;

        if (param.Page != null)
        {
            if (param.Page.Tools != null)
                param.Page.Tools = GetUserButtons(param.Page.Tools, moduleIds, info);
            if (param.Page.Actions != null)
                param.Page.Actions = GetUserActions(param.Page.Actions, moduleIds, info);
            if (param.Page.Columns != null)
                param.Page.Columns = GetUserColumns(param.Page.Columns, moduleIds, info);
        }

        info.Plugins.AddPlugin(param, param.Id, param.Type);
    }

    private static List<ActionInfo> GetUserButtons(List<ActionInfo> buttons, List<string> moduleIds, MenuInfo info)
    {
        var datas = new List<ActionInfo>();
        foreach (var item in buttons)
        {
            if (moduleIds.Contains($"b_{info.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<ActionInfo> GetUserActions(List<ActionInfo> actions, List<string> moduleIds, MenuInfo info)
    {
        var datas = new List<ActionInfo>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{info.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<PageColumnInfo> GetUserColumns(List<PageColumnInfo> columns, List<string> moduleIds, MenuInfo info)
    {
        var datas = new List<PageColumnInfo>();
        foreach (var item in columns)
        {
            if (moduleIds.Contains($"c_{info.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }
}