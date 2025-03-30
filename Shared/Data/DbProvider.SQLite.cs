namespace Known.Data;

class SQLiteProvider(Database db) : DbProvider(db)
{
    internal override string GetTableSql(string dbName)
    {
        return "SELECT name FROM sqlite_master WHERE type='table'";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields, info.Keys);
    }

    internal override string GetTopSql(int size, string text)
    {
        return $"{text} limit {size} offset 0";
    }

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {criteria.PageSize} offset {startNo}";
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, List<string> keys, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [{0}] (", tableName);
        var index = 0;
        foreach (var item in fields)
        {
            var comma = ++index == fields.Count && keys.Count < 2 ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Id}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSQLiteDbType(item);
            if (item.Id == nameof(EntityBase.Id))
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        if (keys.Count > 1)
        {
            var key = string.Join(", ", keys.Select(k => $"[{k}] ASC"));
            sb.AppendLine($"    CONSTRAINT [PK_{tableName}] PRIMARY KEY ({key})");
        }
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetSQLiteDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "datetime";
        //else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
        //    type = "int";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "varchar(50)";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "ntext";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"varchar({item.Length})";
            else
                type = $"nvarchar({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}