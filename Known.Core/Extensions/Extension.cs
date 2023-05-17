namespace Known.Core.Extensions;

public static class Extension
{
    public static object ToTree(this SysOrganization entity)
    {
        return new
        {
            id = entity.Code,
            pid = entity.ParentId,
            code = entity.Code,
            name = entity.Name,
            title = entity.Name,
            open = string.IsNullOrEmpty(entity.ParentId) || entity.ParentId == "0",
            data = entity
        };
    }

    public static DateTime ToDate(this DateTime dateTime, string format = "yyyy-MM-dd")
    {
        var date = dateTime.ToString(format);
        return DateTime.Parse(date);
    }

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
}