using System.Text;
using Known.Extensions;

namespace Known.Blazor;

public interface ICodeService
{
    string GetSQL(EntityInfo info, DatabaseType dbType);
    string GetEntity(EntityInfo info);
    string GetPage(PageInfo info, EntityInfo entity);
    string GetService(PageInfo info, EntityInfo entity);
    string GetRepository(PageInfo info, EntityInfo entity);
}

class CodeService : ICodeService
{
    #region SQL
    public string GetSQL(EntityInfo info, DatabaseType dbType)
    {
        if (info == null)
            return string.Empty;

        var columns = new List<FieldInfo>
        {
            new() { Id = nameof(EntityBase.Id), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateBy), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateTime), Type = FieldType.Date, Required = true },
            new() { Id = nameof(EntityBase.ModifyBy), Type = FieldType.Text, Length = "50" },
            new() { Id = nameof(EntityBase.ModifyTime), Type = FieldType.Date },
            new() { Id = nameof(EntityBase.Version), Type = FieldType.Number, Required = true },
            new() { Id = nameof(EntityBase.Extension), Type = FieldType.Text },
            new() { Id = nameof(EntityBase.AppId), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CompNo), Type = FieldType.Text, Length = "50", Required = true }
        };
        columns.AddRange(info.Fields);

        var maxLength = columns.Select(f => (f.Id ?? "").Length).Max();
        switch (dbType)
        {
            case DatabaseType.Access:
                return GetAccessScript(info.Id, columns, maxLength);
            case DatabaseType.SQLite:
                return GetSQLiteScript(info.Id, columns, maxLength);
            case DatabaseType.SqlServer:
                return GetSqlServerScript(info.Id, columns, maxLength);
            case DatabaseType.Oracle:
                return GetOracleScript(info.Id, columns, maxLength);
            case DatabaseType.MySql:
            case DatabaseType.Npgsql:
                return GetMySqlScript(info.Id, columns, maxLength);
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
        string type;
        if (item.Type == FieldType.Date)
        {
            type = "DateTime";
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
        if (item.Type == FieldType.Date)
        {
            type = "datetime";
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
        sb.AppendLine(") ");
        sb.AppendLine("GO");
        return sb.ToString();
    }

    private static string GetSqlServerDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date)
        {
            type = "[datetime]";
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
        sb.AppendLine("alter table {0}{1} add constraint PK_{0} primary key(Id);", tableName);
        return sb.ToString();
    }

    private static string GetOracleDbType(FieldInfo item)
    {
        string type;
        if (item.Type == FieldType.Date)
            type = "date";
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
        if (item.Type == FieldType.Date)
            type = "datetime";
        else if (item.Type == FieldType.Number)
            type = string.IsNullOrWhiteSpace(item.Length) ? "int" : $"decimal({item.Length})";
        else
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : $"varchar({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private static string GetColumnName(string column, int maxLength)
    {
        column ??= "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
    }
    #endregion

    #region Entity
    public string GetEntity(EntityInfo info)
    {
        if (info == null)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        if (info.IsFlow)
            sb.AppendLine("using Known.WorkFlows;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("/// &lt;summary&gt;");
        sb.AppendLine("/// {0}实体类。", info.Name);
        sb.AppendLine("/// &lt;/summary&gt;");
        sb.AppendLine("public class {0} : {1}", info.Id, info.IsFlow ? "FlowEntity" : "EntityBase");
        sb.AppendLine("{");

        var index = 0;
        foreach (var item in info.Fields)
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
                sb.AppendLine("    [Required]");
            if (!string.IsNullOrWhiteSpace(item.Length))
                sb.AppendLine("    [MaxLength({0})]", item.Length);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Id);
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetCSharpType(FieldInfo item)
    {
        if (item.Type == FieldType.CheckBox || item.Type == FieldType.Switch)
            return "bool";
        else if (item.Type == FieldType.Date)
            return "DateTime";
        else if (item.Type == FieldType.Number)
            return string.IsNullOrWhiteSpace(item.Length) ? "int" : "decimal";

        return "string";
    }
    #endregion

    #region Page
    public string GetPage(PageInfo info, EntityInfo entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine("using {0}.Services;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Pages;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class {0}List : BaseTablePage&lt;{0}&gt;", entity.Id);
        sb.AppendLine("{");
        sb.AppendLine("    private XXXService Service =&gt; new() { CurrentUser = CurrentUser };");
        sb.AppendLine(" ");
        sb.AppendLine("    protected override async Task OnInitPageAsync()");
        sb.AppendLine("    {");
        sb.AppendLine("        await base.OnInitPageAsync();");
        sb.AppendLine("        Table.OnQuery = Service.Query{0}sAsync;", entity.Id);
        sb.AppendLine("    }");
        sb.AppendLine(" ");

        if (info.Tools != null && info.Tools.Length > 0)
        {
            foreach (var item in info.Tools)
            {
                if (item == "New")
                    sb.AppendLine("    [Action] public void New() =&gt; Table.NewForm(Service.Save{0}Async, new {0}());", entity.Id);
                else if (item == "DeleteM")
                    sb.AppendLine("    [Action] public void DeleteM() =&gt; Table.DeleteM(Service.Delete{0}sAsync);", entity.Id);
                else
                    sb.AppendLine("    [Action] public void {0}() =&gt; {{}};", item);
            }
        }

        if (info.Actions != null && info.Actions.Length > 0)
        {
            foreach (var item in info.Actions)
            {
                if (item == "Edit")
                    sb.AppendLine("    [Action] public void Edit({0} row) =&gt; Table.EditForm(Service.Save{0}Async, row);", entity.Id);
                else if (item == "Delete")
                    sb.AppendLine("    [Action] public void Delete({0} row) =&gt; Table.Delete(Service.Delete{0}sAsync, row);", entity.Id);
                else
                    sb.AppendLine("    [Action] public void {0}({1} row) =&gt; {{}};", item, entity.Id);
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion

    #region Service
    public string GetService(PageInfo info, EntityInfo entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine("using {0}.Repositories;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Services;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class XXXService : ServiceBase");
        sb.AppendLine("{");
        sb.AppendLine("    //{0}", entity.Id);
        sb.AppendLine("    public Task&lt;PagingResult&lt;{0}&gt;&gt; Query{0}sAsync(PagingCriteria criteria)", entity.Id);
        sb.AppendLine("    {");
        sb.AppendLine("        return XXXRepository.Query{0}sAsync(Database, criteria);", entity.Id);
        sb.AppendLine("    }");
        if (info.Tools != null && info.Tools.Contains("DeleteM"))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task&lt;Result&gt; Delete{0}sAsync(List&lt;{0}&gt; models)", entity.Id);
            sb.AppendLine("    {");
            sb.AppendLine("        if (models == null || models.Count == 0)");
            sb.AppendLine("            return Result.Error(\"请至少选择一条记录进行操作！\");");
            sb.AppendLine(" ");
            sb.AppendLine("        return await Database.TransactionAsync(\"删除\", async db =&gt;", entity.Id);
            sb.AppendLine("        {");
            sb.AppendLine("            foreach (var item in models)");
            sb.AppendLine("            {");
            sb.AppendLine("                await db.DeleteAsync(item);");
            sb.AppendLine("            }");
            sb.AppendLine("        }, model);");
            sb.AppendLine("    }");
        }
        if (info.Tools != null && info.Tools.Contains("New"))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task&lt;Result&gt; Save{0}Async({0} model)", entity.Id);
            sb.AppendLine("    {");
            sb.AppendLine("        var vr = model.Validate();");
            sb.AppendLine("        if (!vr.IsValid)");
            sb.AppendLine("            return vr;");
            sb.AppendLine(" ");
            sb.AppendLine("        return await Database.TransactionAsync(\"保存\", async db =&gt;", entity.Id);
            sb.AppendLine("        {");
            sb.AppendLine("            await db.SaveAsync(model);");
            sb.AppendLine("        }, model);");
            sb.AppendLine("    }");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion

    #region Repository
    public string GetRepository(PageInfo info, EntityInfo entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Repositories;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class XXXRepository");
        sb.AppendLine("{");
        sb.AppendLine("    //{0}", entity.Id);
        sb.AppendLine("    internal static Task&lt;PagingResult&lt;{0}&gt;&gt; Query{0}sAsync(Database db, PagingCriteria criteria)", entity.Id);
        sb.AppendLine("    {");
        sb.AppendLine("        var sql = \"select * from {0} where CompNo=@CompNo\";", entity.Id);
        sb.AppendLine("        return db.Query{0}Async&lt;{0}&gt;(sql, criteria);", entity.Id);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion
}