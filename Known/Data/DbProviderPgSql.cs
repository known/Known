namespace Known.Data;

class PgSqlProvider(Database db) : DbProvider(db)
{
    //public override string FormatName(string name) => $"\"{name}\"";
    public override object FormatDate(string date) => DateTime.Parse(date);

    internal override string GetTableSql(string dbName)
    {
        return "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE {0} (", tableName);
        foreach (var item in info.Fields)
        {
            var required = item.Required ? " NOT NULL" : "";
            var column = item.Id;
            var type = GetPgSqlDbType(item);
            var line = $"    {column} {type}".TrimEnd();
            sb.AppendLine($"    {line}{required},");
        }
        var keys = string.Join(", ", info.Keys);
        sb.AppendLine($"    PRIMARY KEY({keys})");
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetPgSqlDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date)
            type = "date";
        else if (item.Type == FieldType.DateTime)
            type = "timestamp without time zone";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"character varying({item.Length})";

        return type;
    }
}