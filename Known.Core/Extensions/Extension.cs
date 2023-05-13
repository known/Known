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

    public static T GetValue<T>(this IDictionary dic, string key)
    {
        if (!dic.Contains(key))
            return default;

        var value = dic[key];
        return Utils.ConvertTo<T>(value);
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

    public static Dictionary<string, string> ToParameters(this PagingCriteria criteria, UserInfo user)
    {
        var parameter = new Dictionary<string, string>
        {
            ["AppId"] = user.AppId,
            ["CompNo"] = user.CompNo
        };

        foreach (var item in criteria.Query)
        {
            parameter[item.Id] = item.Value;
        }
        return parameter;
    }

    public static string GetValue(this PagingCriteria criteria, string id)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return string.Empty;

        return query.Value;
    }

    public static T GetValue<T>(this PagingCriteria criteria, string id)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return default;

        return Utils.ConvertTo<T>(query.Value);
    }

    public static void SetType(this PagingCriteria criteria, string id, QueryType type)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return;

        query.Type = type;
    }

    public static bool HasParameter(this PagingCriteria criteria, string id)
    {
        if (criteria.Query == null)
            return false;

        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return false;

        return !string.IsNullOrEmpty(query.Value);
    }
}