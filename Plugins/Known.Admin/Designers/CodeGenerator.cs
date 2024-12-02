namespace Known.Designers;

/// <summary>
/// 代码生成器接口。
/// </summary>
interface ICodeGenerator
{
    /// <summary>
    /// 获取数据库建表脚本。
    /// </summary>
    /// <param name="dbType">数据库类型。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>建表脚本。</returns>
    string GetScript(DatabaseType dbType, EntityInfo entity);

    /// <summary>
    /// 获取实体类代码。
    /// </summary>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>实体类代码。</returns>
    string GetEntity(EntityInfo entity);

    /// <summary>
    /// 获取页面组件代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>页面组件代码。</returns>
    string GetPage(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取业务服务接口代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>业务服务接口代码。</returns>
    string GetIService(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取业务逻辑服务层代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>业务逻辑服务层代码。</returns>
    string GetService(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取数据依赖访问层代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>数据依赖访问层代码。</returns>
    string GetRepository(PageInfo page, EntityInfo entity);
}

class CodeGenerator : ICodeGenerator
{
    #region SQL
    public string GetScript(DatabaseType dbType, EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var columns = new List<FieldInfo>
        {
            new() { Id = nameof(EntityBase.Id), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateBy), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateTime), Type = FieldType.DateTime, Required = true },
            new() { Id = nameof(EntityBase.ModifyBy), Type = FieldType.Text, Length = "50" },
            new() { Id = nameof(EntityBase.ModifyTime), Type = FieldType.DateTime },
            new() { Id = nameof(EntityBase.Version), Type = FieldType.Number, Required = true },
            new() { Id = nameof(EntityBase.Extension), Type = FieldType.Text },
            new() { Id = nameof(EntityBase.AppId), Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CompNo), Type = FieldType.Text, Length = "50", Required = true }
        };
        columns.AddRange(entity.Fields);

        var maxLength = columns.Select(f => (f.Id ?? "").Length).Max();
        switch (dbType)
        {
            case DatabaseType.Access:
                return GetAccessScript(entity.Id, columns, maxLength);
            case DatabaseType.SQLite:
                return GetSQLiteScript(entity.Id, columns, maxLength);
            case DatabaseType.SqlServer:
                return GetSqlServerScript(entity.Id, columns, maxLength);
            case DatabaseType.Oracle:
                return GetOracleScript(entity.Id, columns, maxLength);
            case DatabaseType.MySql:
                return GetMySqlScript(entity.Id, columns, maxLength);
            case DatabaseType.PgSql:
                return GetPgSqlScript(entity.Id, columns, maxLength);
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
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
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
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
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
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
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
        if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            type = "datetime";
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
    #endregion

    #region Entity
    public string GetEntity(EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}实体类。", entity.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public partial class {0} : {1}", entity.Id, entity.IsFlow ? "FlowEntity" : "EntityBase");
        sb.AppendLine("{");

        var index = 0;
        foreach (var item in entity.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var type = GetCSharpType(item);
            if (!item.Required && type != "string")
                type += "?";

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 取得或设置{0}。", item.Name);
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [DisplayName(\"{0}\")]", item.Name);
            if (item.Required)
                sb.AppendLine("    [Required]");
            if (!string.IsNullOrWhiteSpace(item.Length) && type == "string")
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
        else if (item.Type == FieldType.Date || item.Type == FieldType.DateTime)
            return "DateTime";
        else if (item.Type == FieldType.Number)
            return string.IsNullOrWhiteSpace(item.Length) ? "int" : "decimal";

        return "string";
    }
    #endregion

    #region Page
    public string GetPage(PageInfo page, EntityInfo entity)
    {
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine("using {0}.Services;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Pages;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("[Route(\"{0}\")]", entity.PageUrl);
        sb.AppendLine("public class {0}List : BaseTablePage<{1}>", className, entity.Id);
        sb.AppendLine("{");
        sb.AppendLine("    private I{0}Service Service;", className);
        sb.AppendLine(" ");
        sb.AppendLine("    protected override async Task OnInitPageAsync()");
        sb.AppendLine("    {");
        sb.AppendLine("        await base.OnInitPageAsync();");
        sb.AppendLine("        Service = await CreateServiceAsync<I{0}Service>();", className);
        sb.AppendLine("        Table.OnQuery = Service.Query{0}Async;", pluralName);
        sb.AppendLine("    }");
        sb.AppendLine(" ");

        var import = string.Empty;
        var export = string.Empty;
        if (page.Tools != null && page.Tools.Length > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "Import")
                {
                    import = "    public Task Import() => Table.ShowImportAsync();";
                    continue;
                }

                if (item == "Export")
                {
                    export = "    public Task Export() => Table.ExportDataAsync();";
                    continue;
                }

                if (item == "New")
                    sb.AppendLine("    public void New() => Table.NewForm(Service.Save{0}Async, new {1}());", className, entity.Id);
                else if (item == "DeleteM")
                    sb.AppendLine("    public void DeleteM() => Table.DeleteM(Service.Delete{0}Async);", pluralName);
                else
                    sb.AppendLine("    public void {0}() => Table.SelectRows(Service.{0}{1}Async, Language[\"Button.{0}\"]);", item, pluralName);
            }
        }

        if (page.Actions != null && page.Actions.Length > 0)
        {
            foreach (var item in page.Actions)
            {
                if (item == "Edit")
                    sb.AppendLine("    public void Edit({0} row) => Table.EditForm(Service.Save{1}Async, row);", entity.Id, className);
                else if (item == "Delete")
                    sb.AppendLine("    public void Delete({0} row) => Table.Delete(Service.Delete{1}Async, row);", entity.Id, pluralName);
                else
                    sb.AppendLine("    public void {0}({1} row) => {{}};", item, entity.Id);
            }
        }

        if (!string.IsNullOrEmpty(import))
            sb.AppendLine(import);
        if (!string.IsNullOrEmpty(export))
            sb.AppendLine(export);

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetPluralName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var className = DataHelper.GetClassName(name);
        if (!className.EndsWith('y'))
            return className + "s";

        return className.Substring(0, className.Length - 1) + "ies";
    }
    #endregion

    #region Service
    public string GetIService(PageInfo page, EntityInfo entity)
    {
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Services;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("public interface I{0}Service : IService", className);
        sb.AppendLine("{");
        sb.AppendLine("    // {0}", entity.Id);
        sb.AppendLine("    Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria);", entity.Id, pluralName);
        if (page.Tools != null && page.Tools.Length > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "Import" || item == "Export")
                    continue;

                if (item == "DeleteM")
                {
                    sb.AppendLine("    Task<Result> Delete{0}Async(List<{1}> models);", pluralName, entity.Id);
                }
                else
                {
                    sb.AppendLine("    Task<Result> {0}{1}Async(List<{2}> models);", item, pluralName, entity.Id);
                }
            }

            if (page.Tools.Contains("New"))
            {
                sb.AppendLine("    Task<Result> Save{0}Async({1} model);", className, entity.Id);
            }
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion

    #region Service
    public string GetService(PageInfo page, EntityInfo entity)
    {
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("using {0}.Entities;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("namespace {0}.Services;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class {0}Service(Context context) : ServiceBase(context), I{0}Service", className);
        sb.AppendLine("{");
        sb.AppendLine("    // {0}", entity.Id);
        sb.AppendLine("    public Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria)", entity.Id, pluralName);
        sb.AppendLine("    {");
        sb.AppendLine("        return Database.QueryPageAsync<{0}>(criteria);", entity.Id);
        sb.AppendLine("    }");

        if (page.Tools != null && page.Tools.Length > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "Import" || item == "Export")
                    continue;

                if (item == "DeleteM")
                {
                    sb.AppendLine(" ");
                    sb.AppendLine("    public async Task<Result> Delete{0}Async(List<{1}> models)", pluralName, entity.Id);
                    sb.AppendLine("    {");
                    sb.AppendLine("        if (models == null || models.Count == 0)");
                    sb.AppendLine("            return Result.Error(Language.SelectOneAtLeast);");
                    sb.AppendLine(" ");
                    sb.AppendLine("        return await Database.TransactionAsync(Language.Delete, async db =>");
                    sb.AppendLine("        {");
                    sb.AppendLine("            foreach (var item in models)");
                    sb.AppendLine("            {");
                    sb.AppendLine("                await db.DeleteAsync(item);");
                    sb.AppendLine("            }");
                    sb.AppendLine("        });");
                    sb.AppendLine("    }");
                }
                else
                {
                    sb.AppendLine(" ");
                    sb.AppendLine("    public async Task<Result> {0}{1}Async(List<{2}> models)", item, pluralName, entity.Id);
                    sb.AppendLine("    {");
                    sb.AppendLine("        if (models == null || models.Count == 0)");
                    sb.AppendLine("            return Result.Error(Language.SelectOneAtLeast);");
                    sb.AppendLine(" ");
                    sb.AppendLine("        return await Database.TransactionAsync(Language[\"Button.{0}\"], async db =>", item);
                    sb.AppendLine("        {");
                    sb.AppendLine("        });");
                    sb.AppendLine("    }");
                }
            }

            if (page.Tools.Contains("New"))
            {
                sb.AppendLine(" ");
                sb.AppendLine("    public async Task<Result> Save{0}Async({1} model)", className, entity.Id);
                sb.AppendLine("    {");
                sb.AppendLine("        var vr = model.Validate(Context);");
                sb.AppendLine("        if (!vr.IsValid)");
                sb.AppendLine("            return vr;");
                sb.AppendLine(" ");
                sb.AppendLine("        return await Database.TransactionAsync(Language.Save, async db =>", entity.Id);
                sb.AppendLine("        {");
                sb.AppendLine("            await db.SaveAsync(model);");
                sb.AppendLine("        }, model);");
                sb.AppendLine("    }");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion

    #region Repository
    public string GetRepository(PageInfo page, EntityInfo entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Repositories;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class XXXRepository");
        sb.AppendLine("{");
        sb.AppendLine("    //{0}", entity.Id);
        sb.AppendLine("    internal static Task<PagingResult<{0}>> Query{0}sAsync(Database db, PagingCriteria criteria)", entity.Id);
        sb.AppendLine("    {");
        sb.AppendLine("        var sql = \"select * from {0} where CompNo=@CompNo\";", entity.Id);
        sb.AppendLine("        return db.QueryPageAsync<{0}>(sql, criteria);", entity.Id);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
    #endregion
}