using System.Text;
using Known.Extensions;

namespace Known.Designers;

class CodeService
{
    #region SQL
    public string GetSQL(EntityInfo model, DatabaseType dbType)
    {
        var columns = new List<FieldInfo>
        {
            new() { Id = nameof(EntityBase.Id), Type = "Text", Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateBy), Type = "Text", Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateTime), Type = "Date", Required = true },
            new() { Id = nameof(EntityBase.ModifyBy), Type = "Text", Length = "50" },
            new() { Id = nameof(EntityBase.ModifyTime), Type = "Date" },
            new() { Id = nameof(EntityBase.Version), Type = "Number", Required = true },
            new() { Id = nameof(EntityBase.Extension), Type = "Text" },
            new() { Id = nameof(EntityBase.AppId), Type = "Text", Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CompNo), Type = "Text", Length = "50", Required = true }
        };
        columns.AddRange(model.Fields);

        var maxLength = columns.Select(f => (f.Id ?? "").Length).Max();
        switch (dbType)
        {
            case DatabaseType.Access:
                return GetAccessScript(model.Id, columns, maxLength);
            case DatabaseType.SQLite:
                return GetSQLiteScript(model.Id, columns, maxLength);
            case DatabaseType.SqlServer:
                return GetSqlServerScript(model.Id, columns, maxLength);
            case DatabaseType.Oracle:
                return GetOracleScript(model.Id, columns, maxLength);
            case DatabaseType.MySql:
            case DatabaseType.Npgsql:
                return GetMySqlScript(model.Id, columns, maxLength);
            default:
                return string.Empty;
        }
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
        var type = item.Type ?? "";
        if (type == "Date")
        {
            type = "DateTime";
        }
        else if (type == "Number")
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
        var type = item.Type ?? "";
        if (type == "Date")
        {
            type = "datetime";
        }
        else if (type == "Number")
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
        sb.AppendLine(") ");
        sb.AppendLine("GO");
        return sb.ToString();
    }

    private static string GetSqlServerDbType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "Date")
        {
            type = "[datetime]";
        }
        else if (type == "Number")
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
        sb.AppendLine("alter table {0}{1} add constraint PK_{0} primary key(Id);", tableName);
        return sb.ToString();
    }

    private static string GetOracleDbType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "Date")
            type = "date";
        else if (type == "Number")
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
        var type = item.Type ?? "";
        if (type == "Date")
            type = "datetime";
        else if (type == "Number")
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"varchar({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private static string GetColumnName(string column, int maxLength)
    {
        column = column ?? "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
    }
    #endregion

    #region Entity
    public string GetEntity(EntityInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        if (model.IsFlow)
            sb.AppendLine("using Known.WorkFlows;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("/// &lt;summary&gt;");
        sb.AppendLine("/// {0}实体类。", model.Name);
        sb.AppendLine("/// &lt;/summary&gt;");
        sb.AppendLine("public class {0} : {1}", model.Id, model.IsFlow ? "FlowEntity" : "EntityBase");
        sb.AppendLine("{");

        var index = 0;
        foreach (var item in model.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var type = GetCSharpType(item);
            if (!item.Required && type != "string")
                type += "?";

            sb.AppendLine("    /// &lt;summary&gt;");
            sb.AppendLine("    /// 取得或设置{0}。", item.Name);
            sb.AppendLine("    /// &lt;/summary&gt;");
            sb.AppendLine("    [DisplayName(\"{0}\")]", item.Name);
            if (item.Required)
                sb.AppendLine("    [Required(ErrorMessage = \"{0}不能为空！\")]", item.Name);
            if(!string.IsNullOrWhiteSpace(item.Length))
                sb.AppendLine("    [MaxLength({0})]", item.Length);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Id);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GetCSharpType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "CheckBox")
            return "bool";
        else if (type == "Date")
            return "DateTime";
        else if (type == "Number")
            return string.IsNullOrWhiteSpace(item.Length) ? "int" : "decimal";

        return "string";
    }
    #endregion
}