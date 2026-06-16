namespace Known.Data;

class QueryHelper
{
    internal static void SetAutoQuery(Database db, ref string sql, PagingCriteria criteria)
    {
        var querys = new List<QueryInfo>();
        foreach (var item in criteria.Query)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                if (item.Value.Contains('~') && item.Type != QueryType.Between)
                    item.Type = QueryType.Between;
                if (item.Type != QueryType.Between)
                    item.ParamValue = item.Value;
                querys.Add(item);
            }
        }

        var ungrouped = querys.Where(q => string.IsNullOrEmpty(q.GroupId)).ToList();
        var grouped = querys.Where(q => !string.IsNullOrEmpty(q.GroupId)).ToList();

        foreach (var item in ungrouped)
        {
            if (!sql.Contains($"{db.Provider.Prefix}{item.Id}") && item.IsField)
                SetQuery(db, ref sql, criteria, item.Type, item.Id);
            var format = item.Type.ToValueFormat();
            if (!string.IsNullOrWhiteSpace(format))
                item.ParamValue = string.Format(format, item.Value);
        }

        if (grouped.Count > 0)
            BuildOrGroupClause(db, ref sql, criteria, grouped);
    }

    private static void BuildOrGroupClause(Database db, ref string sql, PagingCriteria criteria, List<QueryInfo> groupedQueries)
    {
        var groups = groupedQueries.GroupBy(q => q.GroupId).ToList();
        if (groups.Count == 0) return;

        if (!sql.Contains("where", StringComparison.OrdinalIgnoreCase))
            sql += " where 1=1";

        var orClauses = new List<string>();
        foreach (var group in groups)
        {
            var andClauses = new List<string>();
            foreach (var item in group)
            {
                if (!item.IsField) continue;
                var paramKey = $"{item.Id}_{item.GroupId}";
                var format = item.Type.ToValueFormat();
                var paramValue = !string.IsNullOrWhiteSpace(format)
                               ? string.Format(format, item.Value) : item.ParamValue;
                if (!criteria.HasQuery(paramKey))
                {
                    var qi = criteria.SetQuery(paramKey, item.Type, item.Value);
                    qi.ParamValue = paramValue;
                    qi.GroupId = item.GroupId;
                }
                var clauses = GetConditionClauses(db, criteria, item, paramKey);
                andClauses.AddRange(clauses);
            }
            if (andClauses.Count > 0)
                orClauses.Add("(" + string.Join(" and ", andClauses) + ")");
        }

        if (orClauses.Count > 0)
            sql += " and (" + string.Join(" or ", orClauses) + ")";
    }

    private static List<string> GetConditionClauses(Database db, PagingCriteria criteria, QueryInfo item, string paramKey = null)
    {
        var key = item.Id;
        var keys = key.Split('.');
        if (keys.Length > 1)
            key = keys[1];

        if (!criteria.HasQuery(key))
            return [];

        var field = criteria.GetFieldName(key);
        field = db.Provider?.FormatName(field);
        var param = paramKey != null ? $"@{paramKey}" : $"@{key}";

        return item.Type switch
        {
            QueryType.Between => GetBetweenClauses(db, criteria, field, key, QueryType.GreatEqual, ">=", QueryType.LessEqual, "<="),
            QueryType.BetweenNotEqual => GetBetweenClauses(db, criteria, field, key, QueryType.GreatThan, ">", QueryType.LessThan, "<"),
            QueryType.BetweenLessEqual => GetBetweenClauses(db, criteria, field, key, QueryType.GreatEqual, ">=", QueryType.LessThan, "<"),
            QueryType.BetweenGreatEqual => GetBetweenClauses(db, criteria, field, key, QueryType.GreatThan, ">", QueryType.LessEqual, "<="),
            QueryType.Contain => [db.DatabaseType == DatabaseType.Access ? $"{field} like '{item.Value}'" : $"{field} like {param}"],
            QueryType.NotContain => [db.DatabaseType == DatabaseType.Access ? $"{field} not like '{item.Value}'" : $"{field} not like {param}"],
            QueryType.StartWith => [db.DatabaseType == DatabaseType.Access ? $"{field} like '{item.Value}'" : $"{field} like {param}"],
            QueryType.NotStartWith => [db.DatabaseType == DatabaseType.Access ? $"{field} not like '{item.Value}'" : $"{field} not like {param}"],
            QueryType.EndWith => [db.DatabaseType == DatabaseType.Access ? $"{field} like '{item.Value}'" : $"{field} like {param}"],
            QueryType.NotEndWith => [db.DatabaseType == DatabaseType.Access ? $"{field} not like '{item.Value}'" : $"{field} not like {param}"],
            QueryType.Batch => GetBatchClauses(criteria, field, key),
            QueryType.In => [$"{field} in ('{item.Value.Replace(",", "','")}')"],
            QueryType.NotIn => [$"{field} not in ('{item.Value.Replace(",", "','")}')"],
            _ => [$"{field}{item.Type.ToOperator()}{param}"]
        };
    }

    private static List<string> GetBetweenClauses(Database db, PagingCriteria criteria, string field, string key, QueryType lessType, string lessSymbol, QueryType greatType, string greatSymbol)
    {
        var clauses = new List<string>();
        var paramName = $"L{key}";
        var date = db.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            clauses.Add($"{field}{lessSymbol}{date}");
            query.ParamValue = GetStartDateValue(db, query.Value);
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[0];
            if (!string.IsNullOrWhiteSpace(value))
            {
                clauses.Add($"{field}{lessSymbol}{date}");
                var query1 = criteria.SetQuery(paramName, lessType, value);
                query1.ParamValue = GetStartDateValue(db, value);
            }
        }

        paramName = $"G{key}";
        date = db.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            clauses.Add($"{field}{greatSymbol}{date}");
            query.ParamValue = GetEndDateValue(db, query.Value);
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[1];
            if (!string.IsNullOrWhiteSpace(value))
            {
                clauses.Add($"{field}{greatSymbol}{date}");
                var query1 = criteria.SetQuery(paramName, greatType, value);
                query1.ParamValue = GetEndDateValue(db, value);
            }
        }

        return clauses;
    }

    private static List<string> GetBatchClauses(PagingCriteria criteria, string field, string key)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        var value = query.Value;
        if (string.IsNullOrWhiteSpace(value))
            return [];

        var values = value.Split(',', '，');
        var wheres = new List<string>();
        for (int i = 0; i < values.Length; i++)
        {
            var pkey = $"{key}{i}";
            wheres.Add($"{field}=@{pkey}");
            criteria.SetQuery(pkey, QueryType.Equal, values[i]);
        }
        return [$"({string.Join(" or ", wheres)})"];
    }

    private static void SetQuery(Database db, ref string sql, PagingCriteria criteria, QueryType type, string key, string field = null)
    {
        if (criteria.ExportMode == ExportMode.All)
            return;

        var keys = key.Split('.');
        if (keys.Length > 1)
            key = keys[1];

        if (!criteria.HasQuery(key))
            return;

        if (string.IsNullOrWhiteSpace(field))
            field = criteria.GetFieldName(key);

        if (!sql.Contains("where", StringComparison.OrdinalIgnoreCase))
            sql += " where 1=1";

        field = db.Provider?.FormatName(field);
        switch (type)
        {
            case QueryType.Between:
                SetLessQuery(db, ref sql, criteria, field, key, QueryType.GreatEqual, ">=");
                SetGreatQuery(db, ref sql, criteria, field, key, QueryType.LessEqual, "<=");
                break;
            case QueryType.BetweenNotEqual:
                SetLessQuery(db, ref sql, criteria, field, key, QueryType.GreatThan, ">");
                SetGreatQuery(db, ref sql, criteria, field, key, QueryType.LessThan, "<");
                break;
            case QueryType.BetweenLessEqual:
                SetLessQuery(db, ref sql, criteria, field, key, QueryType.GreatEqual, ">=");
                SetGreatQuery(db, ref sql, criteria, field, key, QueryType.LessThan, "<");
                break;
            case QueryType.BetweenGreatEqual:
                SetLessQuery(db, ref sql, criteria, field, key, QueryType.GreatThan, ">");
                SetGreatQuery(db, ref sql, criteria, field, key, QueryType.LessEqual, "<=");
                break;
            case QueryType.Contain:
                SetLikeQuery(db, ref sql, criteria, field, key, "like");
                break;
            case QueryType.NotContain:
                SetLikeQuery(db, ref sql, criteria, field, key, "not like");
                break;
            case QueryType.StartWith:
                SetLikeQuery(db, ref sql, criteria, field, key, "like");
                break;
            case QueryType.NotStartWith:
                SetLikeQuery(db, ref sql, criteria, field, key, "not like");
                break;
            case QueryType.EndWith:
                SetLikeQuery(db, ref sql, criteria, field, key, "like");
                break;
            case QueryType.NotEndWith:
                SetLikeQuery(db, ref sql, criteria, field, key, "not like");
                break;
            case QueryType.Batch:
                SetBatchQuery(ref sql, criteria, field, key);
                break;
            case QueryType.In:
                SetBatchQuery(ref sql, criteria, field, key, "in");
                break;
            case QueryType.NotIn:
                SetBatchQuery(ref sql, criteria, field, key, "not in");
                break;
            default:
                var operate = type.ToOperator();
                sql += $" and {field}{operate}@{key}";
                break;
        }
    }

    private static void SetLessQuery(Database db, ref string sql, PagingCriteria criteria, string field, string key, QueryType type, string symbol)
    {
        var paramName = $"L{key}";
        var date = db.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.ParamValue = GetStartDateValue(db, query.Value);
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[0];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, type, value);
                query1.ParamValue = GetStartDateValue(db, value);
            }
        }
    }

    private static void SetGreatQuery(Database db, ref string sql, PagingCriteria criteria, string field, string key, QueryType type, string symbol)
    {
        var paramName = $"G{key}";
        var date = db.GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.ParamValue = GetEndDateValue(db, query.Value);
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[1];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, type, value);
                query1.ParamValue = GetEndDateValue(db, value);
            }
        }
    }

    internal static object GetStartDateValue(Database db, string value)
    {
        if (value.Contains(':'))
            return db.Provider.FormatDate(value);

        return db.Provider.FormatDate($"{value} 00:00:00");
    }

    internal static object GetEndDateValue(Database db, string value)
    {
        if (value.Contains(':'))
            return db.Provider.FormatDate(value);

        return db.Provider.FormatDate($"{value} 23:59:59");
    }

    private static void SetLikeQuery(Database db, ref string sql, PagingCriteria criteria, string field, string key, string operate)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        if (db.DatabaseType == DatabaseType.Access)
            sql += $" and {field} {operate} '{query.Value}'";
        else
            sql += $" and {field} {operate} @{key}";
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

    private static void SetBatchQuery(ref string sql, PagingCriteria criteria, string field, string key, string operate)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        var value = query.Value;
        if (string.IsNullOrWhiteSpace(value))
            return;

        var values = value.Replace(",", "','");
        sql += $" and {field} {operate} ('{values}')";
    }
}