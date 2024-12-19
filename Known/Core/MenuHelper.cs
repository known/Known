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
        if (modules == null || modules.Count == 0)
            return [];

        DataHelper.Initialize(modules);

        var routes = DataHelper.GetRouteModules(modules.Select(m => m.Url).ToList());
        if (routes != null && routes.Count > 0)
            modules.AddRange(routes);

        if (user.IsAdmin())
            return modules.ToMenus(true);

        if (moduleIds == null || moduleIds.Count == 0)
            return [];

        var userModules = new List<ModuleInfo>();
        foreach (var item in modules)
        {
            if (!item.Enabled)
                continue;

            if (!moduleIds.Contains(item.Id))
                continue;

            if (userModules.Exists(m => m.Id == item.Id))
                continue;

            AddParentModule(modules, userModules, item);
            item.Buttons = GetUserButtons(moduleIds, item);
            item.Actions = GetUserActions(moduleIds, item);
            item.Columns = GetUserColumns(moduleIds, item);
            userModules.Add(item);
        }
        return userModules.ToMenus(false);
    }

    internal static Task<Result> SaveMenuAsync(MenuInfo info)
    {
        var module = ModuleDB.Modules.FirstOrDefault(m => m.Id == info.Id);
        if (module == null)
        {
            module = new ModuleInfo();
            ModuleDB.Modules.Add(module);
        }
        module.Id = info.Id;
        module.ParentId = info.ParentId;
        module.Code = info.Code;
        module.Name = info.Name;
        module.Icon = info.Icon;
        module.Description = info.Description;
        module.Target = info.Target;
        module.Url = info.Url;
        module.Sort = info.Sort;
        module.Enabled = info.Enabled;
        //module.EntityData = info.EntityData;
        //module.FlowData = info.FlowData;
        module.Page = info.Page;
        module.Form = info.Form;
        ModuleDB.Save();
        return Result.SuccessAsync("保存成功！", info);
    }

    private static void AddParentModule(List<ModuleInfo> modules, List<ModuleInfo> userModules, ModuleInfo item)
    {
        if (!userModules.Exists(m => m.Id == item.ParentId))
        {
            var parent = modules.FirstOrDefault(m => m.Id == item.ParentId);
            if (parent != null)
            {
                userModules.Add(parent);
                AddParentModule(modules, userModules, parent);
            }
        }
    }

    private static List<string> GetUserButtons(List<string> moduleIds, ModuleInfo module)
    {
        var buttons = module.GetToolButtons();
        if (buttons == null || buttons.Count == 0)
            return [];

        var datas = new List<string>();
        foreach (var item in buttons)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<string> GetUserActions(List<string> moduleIds, ModuleInfo module)
    {
        var actions = module.GetTableActions();
        if (actions == null || actions.Count == 0)
            return [];

        var datas = new List<string>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<PageColumnInfo> GetUserColumns(List<string> moduleIds, ModuleInfo module)
    {
        var columns = module.GetPageColumns();
        if (columns == null || columns.Count == 0)
            return null;

        var datas = new List<PageColumnInfo>();
        foreach (var item in columns)
        {
            if (moduleIds.Contains($"c_{module.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }
}