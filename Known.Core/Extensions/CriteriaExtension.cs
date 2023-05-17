namespace Known.Core.Extensions;

public static class CriteriaExtension
{
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