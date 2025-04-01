namespace Known.Data;

class MySqlProvider(Database db) : DbProvider(db)
{
    internal override string GetTableSql(string dbName)
    {
        if (string.IsNullOrWhiteSpace(dbName))
            dbName = GetSchemaName(dbName);

        return @$"SELECT TABLE_NAME AS Id, TABLE_COMMENT AS Name 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA='{dbName}' AND TABLE_TYPE='BASE TABLE'";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields, info.Keys);
    }

    internal override string GetTopSql(int size, string text) => $"{text} limit 0, {size}";

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {startNo}, {criteria.PageSize}";
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, List<string> keys, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table `{0}` (", tableName);
        foreach (var item in fields)
        {
            var required = item.Required ? "not null" : "null";
            var column = $"`{item.Id}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetMySqlDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        if (keys != null && keys.Count > 0)
        {
            var key = string.Join(", ", keys.Select(k => $"`{k}`"));
            sb.AppendLine($"    PRIMARY KEY({key})");
        }
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetMySqlDbType(FieldInfo item)
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
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"varchar({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private string GetSchemaName(string dbName)
    {
        var items = Database.ConnectionString.Split(';');
        foreach (var item in items)
        {
            var names = item?.Split('=');
            if (names != null && names.Length > 1 && (names[0] == "Initial Catalog" || names[0] == "Database"))
            {
                dbName = names[1];
                break;
            }
        }

        return dbName;
    }
}