namespace Known.Data;

class AccessProvider(Database db) : DbProvider(db)
{
    public override string FormatName(string name) => $"`{name}`";

    internal override string GetTableSql(string dbName)
    {
        return "SELECT Name FROM MSysObjects WHERE Type=1 AND Flags=0;";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields);
    }

    internal override string GetTopSql(int size, string text)
    {
        return $"select top {size} t1.* from ({text}) t1";
    }

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        //var order = string.Empty;
        //if (criteria.OrderBys != null)
        //{
        //    order = string.Join(",", criteria.OrderBys);
        //    if (criteria.OrderBys.Length > 1)
        //        return $"{text} order by {order}";
        //}
        //else
        //{
        //    order = "CreateTime";
        //}

        var order1 = $"{order} desc";
        if (order.EndsWith("desc"))
            order1 = order.Replace("desc", "");
        else if (order.EndsWith("asc"))
            order1 = order.Replace("asc", "desc");

        var page = criteria.PageIndex;
        return $@"select t3.* from (
    select top {criteria.PageSize} t2.* from(
        select top {page * criteria.PageSize} t1.* from ({text}) t1 order by t1.{order}
    ) t2 order by t2.{order1}
) t3 order by t3.{order}";
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE `{0}` (", tableName);
        var index = 0;
        foreach (var item in fields)
        {
            var comma = ++index == fields.Count ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"`{item.Id}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetAccessDbType(item);
            if (item.Id == "Id")
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(")");
        sb.AppendLine("GO");
        return sb.ToString();
    }

    private static string GetAccessDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "DateTime";
        else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
            type = "Long";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "Long";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "Long" : $"decimal({item.Length})";
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "LongText";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"VarChar({item.Length})";
            else
                type = $"VarChar({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}