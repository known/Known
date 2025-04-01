namespace Known.Data;

class DMProvider(Database db) : DbProvider(db)
{
    public override string Prefix => ":";

    public override string FormatName(string name) => $"\"{name}\"";
    public override object FormatBoolean(bool value) => value ? 1 : 0;
    public override string GetBooleanSql(string field, bool isTrue) => isTrue ? $"{field}=1" : $"{field}=0";

    internal override string GetTableSql(string dbName)
    {
        return @"SELECT a.TABLE_NAME AS Id, b.COMMENTS AS Name 
FROM ALL_TABLES a 
LEFT JOIN ALL_TAB_COMMENTS b ON b.OWNER=a.OWNER AND b.TABLE_NAME=a.TABLE_NAME 
WHERE a.OWNER NOT IN ('SYS', 'SYSTEM')";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields, info.Keys);
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, List<string> keys, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table {0}(", tableName);
        var index = 0;
        foreach (var item in fields)
        {
            var comma = ++index == fields.Count ? "" : ",";
            var required = item.Required ? "not null" : "null";
            var column = GetColumnName(item.Id, maxLength);
            var type = GetDmDbType(item);
            sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        if (keys != null && keys.Count > 0)
        {
            var key = string.Join(", ", keys);
            sb.AppendLine($"alter table {tableName} add constraint PK_{tableName} primary key({key});");
        }
        return sb.ToString();
    }

    private static string GetDmDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "date";
        //else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
        //    type = "number(8)";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "number(8)";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "number(8)" : $"number({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "varchar2(4000)" : $"varchar2({item.Length})";

        return type;
    }
}