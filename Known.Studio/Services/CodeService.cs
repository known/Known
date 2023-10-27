namespace Known.Studio.Services;

class CodeService
{
    internal static string GetCode(string type, string domain)
    {
        try
        {
            var model = new DomainInfo(domain);
            var attr = System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod;
            var code = typeof(CodeService).InvokeMember($"Get{type}", attr, null, null, new object[] { model });
            return code?.ToString();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string GetSQL(DomainInfo model)
    {
        var columns = new List<FieldInfo>
        {
            new FieldInfo { Code = "Id", Type = "Text", Length = "50", Required = true },
            new FieldInfo { Code = "CreateBy", Type = "Text", Length = "50", Required = true },
            new FieldInfo { Code = "CreateTime", Type = "Date", Required = true },
            new FieldInfo { Code = "ModifyBy", Type = "Text", Length = "50" },
            new FieldInfo { Code = "ModifyTime", Type = "Date" },
            new FieldInfo { Code = "Version", Type = "Number", Required = true },
            new FieldInfo { Code = "Extension", Type = "Text" },
            new FieldInfo { Code = "AppId", Type = "Text", Length = "50", Required = true },
            new FieldInfo { Code = "CompNo", Type = "Text", Length = "50", Required = true }
        };
        columns.AddRange(model.Fields);

        var maxLength = columns.Select(f => (f.Code ?? "").Length).Max();
        var sb = new StringBuilder();
        sb.AppendLine("--Access");
        sb.AppendLine("CREATE TABLE `{0}{1}` (", model.Prefix, model.Code);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"`{item.Code}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetAccessDbType(item);
            if (item.Code == "Id")
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(")");
        sb.AppendLine("GO");

        sb.AppendLine("");
        sb.AppendLine("--MySql");
        sb.AppendLine("create table `{0}{1}` (", model.Prefix, model.Code);
        foreach (var item in columns)
        {
            var required = item.Required ? "not null" : "null";
            var column = $"`{item.Code}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetMySqlDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        sb.AppendLine("    PRIMARY KEY(`Id`)");
        sb.AppendLine(");");

        sb.AppendLine("");
        sb.AppendLine("--SqlLite");
        sb.AppendLine("CREATE TABLE [{0}{1}] (", model.Prefix, model.Code);
        index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Code}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSqlLiteDbType(item);
            if (item.Code == "Id")
                sb.AppendLine($"    {column} {type} {required} PRIMARY KEY{comma}");
            else
                sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");

        sb.AppendLine("");
        sb.AppendLine("--Oracle");
        sb.AppendLine("create table {0}{1}(", model.Prefix, model.Code);
        index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required ? "not null" : "null";
            var column = GetColumnName(item.Code, maxLength);
            var type = GetOracleDbType(item);
            sb.AppendLine($"    {column} {type} {required}{comma}");
        }
        sb.AppendLine(");");
        sb.AppendLine("alter table {0}{1} add constraint PK_{0}{1} primary key(Id);", model.Prefix, model.Code);

        sb.AppendLine("");
        sb.AppendLine("--SqlServer");
        sb.AppendLine("CREATE TABLE [{0}{1}] (", model.Prefix, model.Code);
        foreach (var item in columns)
        {
            var required = item.Required ? "NOT NULL" : "NULL";
            var column = $"[{item.Code}]";
            column = GetColumnName(column, maxLength + 2);
            var type = GetSqlServerDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        sb.AppendLine("    CONSTRAINT [PK_{0}{1}] PRIMARY KEY ([Id] ASC)", model.Prefix, model.Code);
        sb.AppendLine(") ");
        sb.AppendLine("GO");

        return sb.ToString();
    }

    public static string GetEntity(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Entities;", model.Project);
        sb.AppendLine("");
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// {0}实体类。", model.Name);
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class {0} : EntityBase", model.EntityName);
        sb.AppendLine("{");

        var index = 0;
        foreach (var item in model.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var tf = item.Required ? "true" : "false";
            var len = !string.IsNullOrWhiteSpace(item.Length) && !item.Length.Contains(",")
                    ? $", \"1\", \"{item.Length}\"" : "";
            var type = GetCSharpType(item);
            if (!item.Required && type != "string")
                type += "?";

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 取得或设置{0}。", item.Name);
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [Column(\"{0}\", \"\", {1}{2})]", item.Name, tf, len);
            sb.AppendLine("    public {0} {1} {{ get; set; }}", type, item.Code);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GetList(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Pages;", model.Project);
        sb.AppendLine(" ");
        sb.AppendLine("class {0}List : WebGridView<{1}, {0}Form>", model.Code, model.EntityName);
        sb.AppendLine("{");
        sb.AppendLine("    protected override Task<PagingResult<{0}>> OnQueryDataAsync(PagingCriteria criteria)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        return Client.{0}.Query{0}sAsync(criteria);", model.Code);
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    protected override Task InitPageAsync() => base.InitPageAsync();");
        sb.AppendLine("    public override bool CheckAction(ButtonInfo action, {0} item) => base.CheckAction(action, item);", model.EntityName);
        sb.AppendLine(" ");
        sb.AppendLine("    public void New() => ShowForm();");
        sb.AppendLine("    public void DeleteM() => DeleteRows(Client.{0}.Delete{0}sAsync);", model.Code);
        sb.AppendLine("    public void Edit({0} row) => ShowForm(row);", model.EntityName);
        sb.AppendLine("    public void Delete({1} row) => DeleteRow(row, Client.{0}.Delete{0}sAsync);", model.Code, model.EntityName);
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string GetForm(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Pages;", model.Project);
        sb.AppendLine(" ");
        sb.AppendLine("[Dialog(800, 420)]");
        sb.AppendLine("class {0}Form : WebForm<{1}>", model.Code, model.EntityName);
        sb.AppendLine("{");
        sb.AppendLine("    protected override void BuildFields(FieldBuilder<{0}> builder)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        builder.Hidden(f => f.Id);");
        sb.AppendLine("        builder.Table(table =>");
        sb.AppendLine("        {");
        sb.AppendLine("            table.ColGroup(100, null);");
        foreach (var item in model.Fields)
        {
            sb.AppendLine("            table.Tr(attr => table.Field<{0}>(f => f.{1}).Build());", item.Type, item.Code);
        }
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    protected override void BuildButtons(RenderTreeBuilder builder)");
        sb.AppendLine("    {");
        sb.AppendLine("        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);");
        sb.AppendLine("        base.BuildButtons(builder);");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private Task OnSave() => SubmitAsync(Client.{0}.Save{0}Async);", model.Code);
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string GetService(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", model.Project);
        sb.AppendLine(" ");
        sb.AppendLine("class {0}Service : BaseService", model.Code);
        sb.AppendLine("{");
        sb.AppendLine("    public Task<PagingResult<{0}>> Query{1}sAsync(PagingCriteria criteria)", model.EntityName, model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        return {0}Repository.Query{0}sAsync(Database, criteria);", model.Code);
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    public async Task<Result> Delete{0}sAsync(List<{1}> models)", model.Code, model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        if (models == null || models.Count == 0)");
        sb.AppendLine("            return Result.Error(Language.SelectOneAtLeast);");
        sb.AppendLine(" ");
        sb.AppendLine("        return await Database.TransactionAsync(Language.Delete, db =>");
        sb.AppendLine("        {");
        sb.AppendLine("            foreach (var item in models)");
        sb.AppendLine("            {");
        sb.AppendLine("                await db.DeleteAsync(item);");
        sb.AppendLine("            }");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    public async Task<Result> Save{0}Async(dynamic model)", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        var entity = await Database.QueryByIdAsync<{0}>((string)model.Id);", model.EntityName);
        sb.AppendLine("        entity ??= new {0}();", model.EntityName);
        sb.AppendLine("        entity.FillModel(model);");
        sb.AppendLine("        var vr = entity.Validate();");
        sb.AppendLine("        if (!vr.IsValid)");
        sb.AppendLine("            return vr;");
        sb.AppendLine(" ");
        sb.AppendLine("        return await Database.TransactionAsync(Language.Save, db =>");
        sb.AppendLine("        {");
        sb.AppendLine("            await db.SaveAsync(entity);");
        sb.AppendLine("        }, entity);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string GetRepository(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Repositories;", model.Project);
        sb.AppendLine(" ");
        sb.AppendLine("class {0}Repository", model.Code);
        sb.AppendLine("{");
        sb.AppendLine("    internal static Task<PagingResult<{0}>> Query{1}sAsync(Database db, PagingCriteria criteria)", model.EntityName, model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        var sql = \"select * from {0} where CompNo=@CompNo\";", model.EntityName);
        sb.AppendLine("        return db.QueryPageAsync<{0}>(sql, criteria);", model.EntityName);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetColumnName(string column, int maxLength)
    {
        column = column ?? "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
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

    private static object GetAccessDbType(FieldInfo item)
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
            else if (item.Code.StartsWith("Is") || item.Code.EndsWith("Id") || item.Code.EndsWith("No") || item.Code == "CompNo")
                type = $"VarChar({item.Length})";
            else
                type = $"VarChar({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
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

    private static string GetSqlLiteDbType(FieldInfo item)
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
            else if (item.Code.StartsWith("Is") || item.Code.EndsWith("Id") || item.Code.EndsWith("No") || item.Code == "CompNo")
                type = $"varchar({item.Length})";
            else
                type = $"nvarchar({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
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
            else if (item.Code.StartsWith("Is") || item.Code.EndsWith("Id") || item.Code.EndsWith("No") || item.Code == "CompNo")
                type = $"[varchar]({item.Length})";
            else
                type = $"[nvarchar]({item.Length})";
        }

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}