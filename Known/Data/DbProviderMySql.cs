﻿namespace Known.Data;

class MySqlProvider : DbProvider
{
    internal override string GetTableSql(string dbName)
    {
        return $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{dbName}' AND table_type = 'BASE TABLE';";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table `{0}` (", tableName);
        foreach (var item in info.Fields)
        {
            var required = item.Required ? "not null" : "null";
            var column = $"`{item.Id}`";
            var type = GetMySqlDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        var keys = string.Join(", ", info.Keys.Select(k => $"`{k}`"));
        sb.AppendLine($"    PRIMARY KEY({keys})");
        sb.AppendLine(");");
        return sb.ToString();
    }

    internal override string GetTopSql(int size, string text) => $"{text} limit 0, {size}";

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {startNo}, {criteria.PageSize}";
    }

    private static string GetMySqlDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "datetime";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"varchar({item.Length})";

        return type;
    }
}