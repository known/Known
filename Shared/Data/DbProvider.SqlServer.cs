namespace Known.Data;

class SqlServerProvider(Database db) : DbProvider(db)
{
    public override string FormatName(string name) => $"[{name}]";
    //public override object FormatBoolean(bool value) => value ? 1 : 0;
    //public override string GetBooleanSql(string field, bool isTrue) => isTrue ? $"{field}=1" : $"{field}=0";

    internal override string GetTableSql(string dbName)
    {
        return "SELECT TABLE_NAME AS Id, TABLE_NAME AS Name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
    }

    internal override string GetTableScript(string tableName, DbModelInfo info)
    {
        return GetTableScript(tableName, info.Fields, info.Keys);
    }

    internal override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }

    internal static string GetTableScript(string tableName, List<FieldInfo> fields, List<string> keys, int maxLength = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [{0}] (", tableName);
        foreach (var item in fields)
        {
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Id}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSqlServerDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        if (keys != null && keys.Count > 0)
        {
            var key = string.Join(", ", keys.Select(k => $"[{k}] ASC"));
            sb.AppendLine($"    CONSTRAINT [PK_{tableName}] PRIMARY KEY ({key})");
        }
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetSqlServerDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "[datetime]";
        //else if (item.Id == nameof(EntityBase.Id) && Config.App.NextIdType == NextIdType.AutoInteger)
        //    type = "[int]";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "[varchar](50)";
        else if (item.Type == FieldType.Integer)
            type = "[int]";
        else if (item.Type == FieldType.Number)
            type = "[decimal](18,5)";
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "[ntext]";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"[varchar]({item.Length})";
            else
                type = $"[nvarchar]({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}