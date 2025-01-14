namespace Known.Data;

class SqlServerProvider(Database db) : DbProvider(db)
{
    public override string FormatName(string name) => $"[{name}]";

    internal override string GetTableSql(string dbName)
    {
        return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [{0}] (", tableName);
        foreach (var item in info.Fields)
        {
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Id}]";
            var type = GetSqlServerDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        var keys = string.Join(", ", info.Keys.Select(k => $"[{k}] ASC"));
        sb.AppendLine($"    CONSTRAINT [PK_{tableName}] PRIMARY KEY ({keys})");
        sb.AppendLine(");");
        return sb.ToString();
    }

    internal override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }

    private static string GetSqlServerDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
        {
            type = "[datetime]";
        }
        else if (item.Type == FieldType.Number)
        {
            type = string.IsNullOrWhiteSpace(item.Length) ? "[int]" : $"[decimal]({item.Length})";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "[ntext]";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"[varchar]({item.Length})";
            else
                type = $"[nvarchar]({item.Length})";
        }

        return type;
    }
}