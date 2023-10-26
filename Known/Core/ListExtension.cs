namespace Known.Core;

static class ListExtension
{
    internal static List<MenuInfo> ToMenus(this List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return new List<MenuInfo>();

        return modules.Select(m => new MenuInfo(m.Id, m.Name, m.Icon, m.Description)
        {
            ParentId = m.ParentId,
            Code = m.Code,
            Target = m.Target,
            Sort = m.Sort,
            Buttons = m.Buttons,
            Actions = m.Actions,
            Columns = m.Columns
        }).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
}