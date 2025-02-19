namespace Known.Data;

class OracleProvider(Database db) : DbProvider(db)
{
    public override string Prefix => ":";

    public override string GetDateSql(string name, bool withTime = true)
    {
        var format = "yyyy-mm-dd";
        if (withTime)
            format += " hh24:mi:ss";
        return $"to_date(:{name},'{format}')";
    }

    internal override string GetTableSql(string dbName)
    {
        return "SELECT table_name FROM user_tables;";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table {0}(", tableName);
        var index = 0;
        foreach (var item in info.Fields)
        {
            var comma = ++index == info.Fields.Count ? "" : ",";
            var required = item.Required ? "not null" : "null";
            var column = item.Id;
            var type = item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger
                     ? "number(8)"
                     : GetOracleDbType(item);
            sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        var keys = string.Join(",", info.Keys);
        sb.AppendLine("alter table {0} add constraint PK_{0} primary key({1});", tableName, keys);
        return sb.ToString();
    }

    private static string GetOracleDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "date";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "number(8)" : $"number({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "varchar2(4000)" : $"varchar2({item.Length})";

        return type;
    }
}