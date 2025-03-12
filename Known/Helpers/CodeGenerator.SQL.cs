namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetScript(DatabaseType dbType, EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var columns = TypeHelper.GetBaseFields();
        columns.AddRange(entity.Fields);

        var maxLength = columns.Select(f => (f.Id ?? "").Length).Max();
        return dbType switch
        {
            DatabaseType.Access => GetAccessScript(entity.Id, columns, maxLength),
            DatabaseType.SQLite => GetSQLiteScript(entity.Id, columns, maxLength),
            DatabaseType.SqlServer => GetSqlServerScript(entity.Id, columns, maxLength),
            DatabaseType.Oracle => GetOracleScript(entity.Id, columns, maxLength),
            DatabaseType.MySql => GetMySqlScript(entity.Id, columns, maxLength),
            DatabaseType.PgSql => GetPgSqlScript(entity.Id, columns, maxLength),
            _ => string.Empty,
        };
    }

    private static string GetAccessScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE `{0}` (", tableName);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"`{item.Id}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetAccessDbType(item);
            if (item.Id == "Id")
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(")");
        sb.AppendLine("GO");
        return sb.ToString();
    }

    private static string GetAccessDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
        {
            type = "DateTime";
        }
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
        {
            type = "Long";
        }
        else if (item.Type == FieldType.Number)
        {
            type = string.IsNullOrWhiteSpace(item.Length) ? "Long" : $"decimal({item.Length})";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "LongText";
            else if (item.Id.StartsWith("Is") || item.Id.EndsWith("Id") || item.Id.EndsWith("No") || item.Id == "CompNo")
                type = $"VarChar({item.Length})";
            else
                type = $"VarChar({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private static string GetSQLiteScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [{0}] (", tableName);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Id}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSQLiteDbType(item);
            if (item.Id == "Id")
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetSQLiteDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
        {
            type = "datetime";
        }
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
        {
            type = "int";
        }
        else if (item.Type == FieldType.Number)
        {
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        }
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

    private static string GetSqlServerScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [{0}] (", tableName);
        foreach (var item in columns)
        {
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Id}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSqlServerDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        sb.AppendLine("    CONSTRAINT [PK_{0}] PRIMARY KEY ([Id] ASC)", tableName);
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetSqlServerDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
        {
            type = "[datetime]";
        }
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
        {
            type = "[int]";
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

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private static string GetOracleScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table {0}(", tableName);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "not null" : "null";
            var column = GetColumnName(item.Id, maxLength);
            var type = GetOracleDbType(item);
            sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        sb.AppendLine("alter table {0} add constraint PK_{0} primary key(Id);", tableName);
        return sb.ToString();
    }

    private static string GetOracleDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "date";
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

    private static string GetMySqlScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("create table `{0}` (", tableName);
        foreach (var item in columns)
        {
            var required = item.Required ? "not null" : "null";
            var column = $"`{item.Id}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetMySqlDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        sb.AppendLine("    PRIMARY KEY(`Id`)");
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetMySqlDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "datetime";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "int";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"varchar({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private static string GetPgSqlScript(string tableName, List<FieldInfo> columns, int maxLength)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE {0} (", tableName);
        foreach (var item in columns)
        {
            var required = item.Required ? " NOT NULL" : "";
            var column = $"{item.Id}";
            column = GetColumnName(column, maxLength + 2);
            var type = GetPgSqlDbType(item);
            var line = $"    {column} {type}".TrimEnd();
            sb.AppendLine($"    {line}{required},");
        }
        sb.AppendLine("    PRIMARY KEY(Id)");
        sb.AppendLine(");");
        return sb.ToString();
    }

    private static string GetPgSqlDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date)
            type = "date";
        else if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            type = "int";
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

    private static string GetColumnName(string column, int maxLength)
    {
        column ??= "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
    }
}