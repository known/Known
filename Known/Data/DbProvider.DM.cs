namespace Known.Data;

class DMProvider(Database db) : DbProvider(db)
{
    public override string Prefix => ":";

    public override string FormatName(string name) => $"\"{name}\"";

    internal override string GetTableSql(string dbName)
    {
        return "SELECT table_name FROM all_tables;";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields);
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> columns, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table {0}(", tableName);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "not null" : "null";
            var column = GetColumnName(item.Id, maxLength);
            var type = GetDmDbType(item);
            sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        sb.AppendLine("alter table {0} add constraint PK_{0} primary key(Id);", tableName);
        return sb.ToString();
    }

    private static string GetDmDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "date";
        else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
            type = "number(8)";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "number(8)";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "number(8)" : $"number({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "varchar2(4000)" : $"varchar2({item.Length})";

        return type;
    }
}