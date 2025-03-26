namespace Known.Data;

class OracleProvider(Database db) : DbProvider(db)
{
    public override string Prefix => ":";
    public override object FormatBoolean(bool value) => value ? 1 : 0;
    public override string GetBooleanSql(string field, bool isTrue) => isTrue ? $"{field}=1" : $"{field}=0";

    public override string GetDateSql(string name, bool withTime = true)
    {
        var format = "yyyy-mm-dd";
        if (withTime)
            format += " hh24:mi:ss";
        return $"to_date(:{name},'{format}')";
    }

    internal override string GetTableSql(string dbName)
    {
        return "SELECT table_name FROM user_tables";
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
            var type = GetOracleDbType(item);
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

    private static string GetOracleDbType(FieldInfo item)
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

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}