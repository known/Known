namespace Known.Data;

class PgSqlProvider(Database db) : DbProvider(db)
{
    //public override string FormatName(string name) => $"\"{name}\"";
    public override object FormatDate(string date) => DateTime.Parse(date);

    internal override string GetTableSql(string dbName)
    {
        return "SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields, info.Keys);
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, List<string> keys, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE {0} (", tableName);
        foreach (var item in fields)
        {
            var required = item.Required ? " NOT NULL" : "";
            var column = item.Id;
            column = GetColumnName(column, maxLength + 2);
            var type = GetPgSqlDbType(item);
            var line = $"    {column} {type}".TrimEnd();
            sb.AppendLine($"    {line}{required},");
        }
        if (keys != null && keys.Count > 0)
        {
            var key = string.Join(", ", keys);
            sb.AppendLine($"    PRIMARY KEY({key})");
        }
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetPgSqlDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date)
            type = "date";
        else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
            type = "int";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "character varying(50)";
        else if (item.Type == FieldType.DateTime)
            type = "timestamp without time zone";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"character varying({item.Length})";

        if (type.Length < 24)
            type += new string(' ', 24 - type.Length);

        return type;
    }
}