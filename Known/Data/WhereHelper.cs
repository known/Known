namespace Known.Data;

class WhereHelper
{
    internal static void SetAutoQuery(ref string sql, SqlBuilder builder, PagingCriteria criteria)
    {
        var querys = new List<QueryInfo>();
        foreach (var item in criteria.Query)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                if (item.Value.Contains('~') && item.Type != QueryType.Between)
                    item.Type = QueryType.Between;
                item.ParamValue = item.Value;
                querys.Add(item);
            }
        }
        foreach (var item in querys)
        {
            if (!sql.Contains($"@{item.Id}"))
                SetQuery(ref sql, builder, criteria, item.Type, item.Id);
        }
    }

    private static void SetQuery(ref string sql, SqlBuilder builder, PagingCriteria criteria, QueryType type, string key, string field = null)
    {
        if (criteria.ExportMode == ExportMode.All)
            return;

        field ??= key;
        var keys = key.Split('.');
        if (keys.Length > 1)
            key = keys[1];

        if (!criteria.HasQuery(key))
            return;

        if (criteria.Fields.ContainsKey(key))
            field = criteria.Fields[key];

        field = builder.FormatName(field);
        switch (type)
        {
            case QueryType.Equal:
                sql += $" and {field}=@{key}";
                break;
            case QueryType.NotEqual:
                sql += $" and {field}<>@{key}";
                break;
            case QueryType.LessThan:
                sql += $" and {field}<@{key}";
                break;
            case QueryType.LessEqual:
                sql += $" and {field}<=@{key}";
                break;
            case QueryType.GreatThan:
                sql += $" and {field}>@{key}";
                break;
            case QueryType.GreatEqual:
                sql += $" and {field}>=@{key}";
                break;
            case QueryType.Between:
                SetLessQuery(ref sql, builder, criteria, field, key, ">=");
                SetGreatQuery(ref sql, builder, criteria, field, key, "<=");
                break;
            case QueryType.BetweenNotEqual:
                SetLessQuery(ref sql, builder, criteria, field, key, ">");
                SetGreatQuery(ref sql, builder, criteria, field, key, "<");
                break;
            case QueryType.BetweenLessEqual:
                SetLessQuery(ref sql, builder, criteria, field, key, ">=");
                SetGreatQuery(ref sql, builder, criteria, field, key, "<");
                break;
            case QueryType.BetweenGreatEqual:
                SetLessQuery(ref sql, builder, criteria, field, key, ">");
                SetGreatQuery(ref sql, builder, criteria, field, key, "<=");
                break;
            case QueryType.Contain:
                SetLikeQuery(ref sql, builder, criteria, field, key, "%{0}%");
                break;
            case QueryType.StartWith:
                SetLikeQuery(ref sql, builder, criteria, field, key, "{0}%");
                break;
            case QueryType.EndWith:
                SetLikeQuery(ref sql, builder, criteria, field, key, "%{0}");
                break;
            case QueryType.Batch:
                SetBatchQuery(ref sql, criteria, field, key);
                break;
            default:
                break;
        }
    }

    private static void SetLessQuery(ref string sql, SqlBuilder builder, PagingCriteria criteria, string field, string key, string symbol)
    {
        var paramName = $"L{key}";
        var date = builder.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.ParamValue = $"{query.Value} 00:00:00";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[0];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, value);
                query1.ParamValue = $"{value} 00:00:00";
            }
        }
    }

    private static void SetGreatQuery(ref string sql, SqlBuilder builder, PagingCriteria criteria, string field, string key, string symbol)
    {
        var paramName = $"G{key}";
        var date = builder.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.ParamValue = $"{query.Value} 23:59:59";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[1];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, value);
                query1.ParamValue = $"{value} 23:59:59";
            }
        }
    }

    private static void SetLikeQuery(ref string sql, SqlBuilder builder, PagingCriteria criteria, string field, string key, string format)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        query.ParamValue = string.Format(format, query.Value);
        if (builder.DatabaseType == DatabaseType.Access)
            sql += $" and {field} like '{query.Value}'";
        else
            sql += $" and {field} like @{key}";
    }

    private static void SetBatchQuery(ref string sql, PagingCriteria criteria, string field, string key)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        var value = query.Value;
        if (string.IsNullOrWhiteSpace(value))
            return;

        var values = value.Split(',', '，');
        var wheres = new List<string>();
        for (int i = 0; i < values.Length; i++)
        {
            var pkey = $"{key}{i}";
            wheres.Add($"{field}=@{pkey}");
            criteria.SetQuery(pkey, QueryType.Equal, values[i]);
        }
        var where = string.Join(" or ", wheres);
        sql += $" and ({where})";
    }
}