namespace Known.Razor;

class QueryContext : FieldContext
{
    internal List<QueryInfo> GetData()
    {
        var infos = new List<QueryInfo>();
        foreach (var item in Fields)
        {
            var value = item.Value.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var query = new QueryInfo(item.Key, value);
                infos.Add(query);
            }
        }
        return infos;
    }

    internal static List<QueryInfo> GetData(object query)
    {
        var dics = Utils.MapTo<Dictionary<string, string>>(query);
        if (dics == null || dics.Count == 0)
            return null;

        var infos = new List<QueryInfo>();
        foreach (var item in dics)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
                infos.Add(new QueryInfo(item.Key, item.Value));
        }
        return infos;
    }
}