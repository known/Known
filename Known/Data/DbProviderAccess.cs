﻿namespace Known.Data;

class AccessProvider : DbProvider
{
    public override string FormatName(string name) => $"`{name}`";

    internal override string GetTableSql(string dbName)
    {
        return "SELECT Name FROM MSysObjects WHERE Type=1 AND Flags=0;";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE `{0}` (", tableName);
        var index = 0;
        foreach (var item in info.Fields)
        {
            var comma = ++index == info.Fields.Count && info.Keys.Count < 2 ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"`{item.Id}`";
            var type = GetAccessDbType(item);
            if (item.Id == nameof(EntityBase.Id))
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        if (info.Keys.Count > 1)
        {
            var keys = string.Join(", ", info.Keys.Select(k => $"`{k}`"));
            sb.AppendLine($"    CONSTRAINT `PK_{tableName}` PRIMARY KEY ({keys})");
        }
        sb.AppendLine(")");
        sb.AppendLine("GO");
        return sb.ToString();
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

    private static string GetAccessDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
        {
            type = "DateTime";
        }
        else if (item.Type == FieldType.Number)
        {
            type = string.IsNullOrWhiteSpace(item.Length) ? "Long" : $"decimal({item.Length})";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "LongText";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"VarChar({item.Length})";
            else
                type = $"VarChar({item.Length})";
        }

        return type;
    }
}