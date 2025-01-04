namespace Known.Core;

/// <summary>
/// 菜单帮助者类。
/// </summary>
public sealed class MenuHelper
{
    private MenuHelper() { }

    /// <summary>
    /// 获取用户菜单信息列表。
    /// </summary>
    /// <param name="user">当前用户信息。</param>
    /// <param name="modules">全部模块信息列表。</param>
    /// <param name="moduleIds">当前用户角色模块ID列表。</param>
    /// <returns></returns>
    public static List<MenuInfo> GetUserMenus(UserInfo user, List<ModuleInfo> modules, List<string> moduleIds = null)
    {
        if (user == null || modules == null || modules.Count == 0)
            return [];

        // 定义新列表，在新列表中添加路由模块，不污染原模块列表
        var allModules = new List<ModuleInfo>();
        allModules.AddRange(modules);

        // 添加路由模块
        var urls = allModules.Select(m => m.Url).ToList();
        var ids = user.IsAdmin() ? null : moduleIds;
        var routes = DataHelper.GetRouteModules(urls, ids);
        if (routes != null && routes.Count > 0)
            allModules.AddRange(routes);

        // 如果是管理员，返回所有菜单
        if (user.IsAdmin())
            return allModules.ToMenus();

        if (moduleIds == null || moduleIds.Count == 0)
            return [];

        // 如果是角色用户，根据用户角色模块ID列表返回菜单
        var userModules = new List<ModuleInfo>();
        foreach (var item in allModules)
        {
            if (!item.Enabled)
                continue;

            if (!moduleIds.Contains(item.Id))
                continue;

            if (userModules.Exists(m => m.Id == item.Id))
                continue;

            AddParentModule(userModules, item);
            userModules.Add(item);
        }
        return userModules.ToMenus();
    }

    private static void AddParentModule(List<ModuleInfo> userModules, ModuleInfo item)
    {
        // 如果父模块不存在，则添加父模块
        if (!userModules.Exists(m => m.Id == item.ParentId))
        {
            var parent = AppData.GetModule(item.ParentId);
            if (parent != null)
            {
                userModules.Add(parent);
                AddParentModule(userModules, parent);
            }
        }
    }
}