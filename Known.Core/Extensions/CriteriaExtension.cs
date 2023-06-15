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

    public static string GetQueryValue(this PagingCriteria criteria, string id)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return string.Empty;

        return query.Value;
    }

    public static T GetQueryValue<T>(this PagingCriteria criteria, string id)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return default;

        return Utils.ConvertTo<T>(query.Value);
    }

    public static void SetQueryType(this PagingCriteria criteria, string id, QueryType type)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return;

        query.Type = type;
    }

    public static bool HasQuery(this PagingCriteria criteria, string id)
    {
        if (criteria.Query == null)
            return false;

        var query = criteria.Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return false;

        return !string.IsNullOrEmpty(query.Value);
    }

    public static T GetParameter<T>(this PagingCriteria criteria, string id)
    {
        if (criteria.Parameters == null)
            return default;

        if (!criteria.Parameters.ContainsKey(id))
            return default;

        var param = criteria.Parameters[id];
        return Utils.ConvertTo<T>(param);
    }

    public static bool CheckParameter<T>(this PagingCriteria criteria, string id, T value)
    {
        var data = criteria.GetParameter<T>(id);
        return data.Equals(value);
    }
}