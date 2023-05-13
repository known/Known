using System.Text;
using Known.Studio.Models;

namespace Known.Studio.Services;

static class ModelExtensions
{
    internal static void AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        var value = string.Format(format, args);
        sb.AppendLine(value);
    }
}

class CodeService
{
    internal static string GetCode(string type, string domain)
    {
        var model = new DomainInfo(domain);
        switch (type)
        {
            case "SQL":
                return GetSQLCode(model);
            case "Entity":
                return GetEntityCode(model);
            case "Service":
                return GetServiceCode(model);
            case "ListCS":
                return GetListCSCode(model);
            case "FormCS":
                return GetFormCSCode(model);
            case "ListRazor":
                return GetListRazorCode(model);
            case "FormRazor":
                return GetFormRazorCode(model);
            default:
                return string.Empty;
        }
    }

    private static string GetSQLCode(DomainInfo model)
    {
        var columns = new List<FieldInfo>();
        columns.Add(new FieldInfo { Code = "Id", Type = "Text", Length = "50", Required = true });
        columns.Add(new FieldInfo { Code = "CreateBy", Type = "Text", Length = "50", Required = true });
        columns.Add(new FieldInfo { Code = "CreateTime", Type = "Date", Required = true });
        columns.Add(new FieldInfo { Code = "ModifyBy", Type = "Text", Length = "50" });
        columns.Add(new FieldInfo { Code = "ModifyTime", Type = "Date" });
        columns.Add(new FieldInfo { Code = "Version", Type = "Number", Required = true });
        columns.Add(new FieldInfo { Code = "Extension", Type = "Text" });
        columns.Add(new FieldInfo { Code = "AppId", Type = "Text", Length = "50", Required = true });
        columns.Add(new FieldInfo { Code = "CompNo", Type = "Text", Length = "50", Required = true });
        columns.AddRange(model.Fields);

        var maxLength = columns.Select(f => (f.Code ?? "").Length).Max();
        var sb = new StringBuilder();
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
        var index = 0;
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

    private static string GetEntityCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.{0}.Entities", model.Project);
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// {0}实体类。", model.Name);
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public class {0} : EntityBase", model.EntityName);
        sb.AppendLine("    {");

        var index = 0;
        foreach (var item in model.Fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var tf = item.Required ? "true" : "false";
            var len = !string.IsNullOrWhiteSpace(item.Length) && !item.Length.Contains(",")
                    ? $", \"1\", \"{item.Length}\"" : "";
            var type = GetCSharpType(item);
            if (!item.Required && type != "string?")
                type += "?";

            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// 取得或设置{0}。", item.Name);
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        [Column(\"{0}\", \"\", {1}{2})]", item.Name, tf, len);
            sb.AppendLine("        public {0} {1} {{ get; set; }}", type, item.Code);
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GetServiceCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Known.{0}.Entities;", model.Project);
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.{0}", model.Project);
        sb.AppendLine("{");
        sb.AppendLine("    class {0}Service : ServiceBase", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        public PagingResult<{0}> Query{1}s(PagingCriteria criteria)", model.EntityName, model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return Repository.Query{0}s(Database, criteria);", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        public Result Delete{0}s(List<{1}> models)", model.Code, model.EntityName);
        sb.AppendLine("        {");
        sb.AppendLine("            if (models == null || models.Count == 0)");
        sb.AppendLine("                return Result.Error(Language.SelectOneAtLeast);");
        sb.AppendLine(" ");
        sb.AppendLine("            return Database.Transaction(Language.Delete, db =>");
        sb.AppendLine("            {");
        sb.AppendLine("                foreach (var item in models)");
        sb.AppendLine("                {");
        sb.AppendLine("                    db.Delete(item);");
        sb.AppendLine("                }");
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        public Result Save{0}(dynamic model)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var entity = Database.QueryById<{0}>((string)model.Id);", model.EntityName);
        sb.AppendLine("            if (entity == null)");
        sb.AppendLine("                entity = new {0}();", model.EntityName);
        sb.AppendLine(" ");
        sb.AppendLine("            entity.FillModel(model);");
        sb.AppendLine("            var vr = entity.Validate();");
        sb.AppendLine("            if (!vr.IsValid)");
        sb.AppendLine("                return vr;");
        sb.AppendLine(" ");
        sb.AppendLine("            Database.Save(entity);");
        sb.AppendLine("            return Result.Success(\"保存成功！\", entity.Id);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    class {0}Repository", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        public PagingResult<{0}> Query{1}s(Database db, PagingCriteria criteria)", model.EntityName, model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var sql = \"select * from {0} where CompNo=@CompNo\";", model.EntityName);
        foreach (var item in model.Fields)
        {
            if (item.IsQuery)
            {
                sb.AppendLine("            db.SetQuery(ref sql, criteria, QueryType.Contain, \"{0}\");", item.Code);
            }
        }
        sb.AppendLine("            return db.QueryPage<{0}>(sql, criteria);", model.EntityName);
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetListCSCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Known.Razor;");
        sb.AppendLine("using Known.{0}.Entities;", model.Project);
        sb.AppendLine("using Microsoft.AspNetCore.Components.Rendering;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.{0}.Pages", model.Project);
        sb.AppendLine("{");
        sb.AppendLine("    public class {0}List : WebGridView<{1}>", model.Code, model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        protected override Task<PagingResult<{0}>> OnQueryData(PagingCriteria criteria)", model.EntityName);
        sb.AppendLine("        {");
        sb.AppendLine("            return Client.Query{0}sAsync(criteria);", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        protected override void BuildTools(RenderTreeBuilder builder)");
        sb.AppendLine("        {");
        sb.AppendLine("            builder.ButtonNew(Callback(e => ShowForm()));");
        sb.AppendLine("            builder.ButtonDeleteM(Callback(e => OnDeleteM(Client.Delete{0}sAsync)));", model.Code);
        sb.AppendLine("            builder.ButtonImport(Callback(OnImport));");
        sb.AppendLine("            builder.ButtonExport(Callback(OnExport));");
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        protected override void BuildFields(RenderTreeBuilder builder)");
        sb.AppendLine("        {");
        sb.AppendLine("            builder.Action(80, RenderAction(BuildAction));");
        foreach (var item in model.Fields)
        {
            var query = item.IsQuery ? ", true" : "";
            sb.AppendLine("            builder.Field<{0}>(\"{1}\", nameof({2}.{3}){4});", item.Type, item.Name, model.EntityName, item.Code, query);
        }
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        private void BuildAction(AuthRenderBuilder builder, {0} row)", model.EntityName);
        sb.AppendLine("        {");
        sb.AppendLine("            builder.LinkView(Callback(e => OnDetail(row)));");
        sb.AppendLine("            builder.LinkEdit(Callback(e => ShowForm(row)));");
        sb.AppendLine("            builder.LinkDelete(Callback(e => OnDelete(row, Client.Delete{0}sAsync)));", model.Code, model.EntityName);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        private void ShowForm({0} model = null)", model.EntityName);
        sb.AppendLine("        {");
        sb.AppendLine("            var action = model == null ? \"新增\" : \"编辑\";");
        sb.AppendLine("            model ??= new {0} {{ }};", model.EntityName);
        sb.AppendLine("            grid.ShowForm<{0}Form>($\"{{action}}{1}\", 500, 380, model);", model.Code, model.Name);
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetFormCSCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Known.Razor;");
        sb.AppendLine("using Microsoft.AspNetCore.Components.Rendering;");
        sb.AppendLine("");
        sb.AppendLine("namespace Known.{0}.Pages", model.Project);
        sb.AppendLine("{");
        sb.AppendLine("    public class {0}Form : WebForm<{1}>", model.Code, model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        protected override void BuildFields(RenderTreeBuilder builder)");
        sb.AppendLine("        {");
        sb.AppendLine("            builder.Hidden(nameof({0}.Id));", model.EntityName);
        sb.AppendLine("            builder.Table(attr =>");
        sb.AppendLine("            {");
        sb.AppendLine("                builder.ColGroup(100, null);");
        foreach (var item in model.Fields)
        {
            var required = item.Required ? ", true" : "";
            sb.AppendLine("                builder.Tr(attr => builder.Field<{0}>(f => f.Field(\"{1}\", nameof({2}.{3}){4})));", item.Type, item.Name, model.EntityName, item.Code, required);
        }
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        protected override void BuildButtons(RenderTreeBuilder builder)");
        sb.AppendLine("        {");
        sb.AppendLine("            builder.ButtonSave(Callback(OnSave), !ReadOnly);");
        sb.AppendLine("            builder.ButtonClose(Callback(OnCancel));");
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        protected override OnSave()");
        sb.AppendLine("        {");
        sb.AppendLine("            SubmitAsync(Client.Save{0}Async);", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetListRazorCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@namespace Known.{0}.Pages", model.Project);
        sb.AppendLine("@inherits BasePage");
        sb.AppendLine(" ");
        sb.AppendLine("<DataGridView @ref=\"grid\" OnQuery=\"OnQueryData\">");
        sb.AppendLine("    <Tools>");
        sb.AppendLine("        <Button Text=\"新增\" Icon=\"fa fa-plus\" OnClick=\"OnNew\" />");
        sb.AppendLine("    </Tools>");
        sb.AppendLine("    <Fields>");
        sb.AppendLine("        <Field Label=\"操作\" Id=\"Action\" Width=\"60\" Context=\"row\">");
        sb.AppendLine("            <span class=\"link\" @onclick=\"e => OnEdit(({0})row)\">编辑</span>", model.EntityName);
        sb.AppendLine("            <span class=\"link\" @onclick=\"e => OnDelete(({0})row)\">删除</span>", model.EntityName);
        sb.AppendLine("        </Field>");
        foreach (var item in model.Fields)
        {
            var query = item.IsQuery ? " IsQuery=\"true\"" : "";
            sb.AppendLine("        <{0} Label=\"{1}\" Id=\"{2}\"{3} />", item.Type, item.Name, item.Code, query);
        }
        sb.AppendLine("    </Fields>");
        sb.AppendLine("</DataGridView>");
        sb.AppendLine(" ");
        sb.AppendLine("@code {");
        sb.AppendLine("    private DataGridView<{0}> grid;", model.EntityName);
        sb.AppendLine(" ");
        sb.AppendLine("    private PagingResult<{0}> OnQueryData(PagingCriteria criteria)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        return Service.Query{0}s(criteria);", model.Code);
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private void OnNew()");
        sb.AppendLine("    {");
        sb.AppendLine("        ShowForm(\"新增\", new {0}());", model.EntityName);
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private void OnEdit({0} model)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        ShowForm(\"编辑\", model);");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private void OnDelete({0} model)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        UI.Confirm($\"确定要删除记录？\", () =>");
        sb.AppendLine("        {");
        sb.AppendLine("            var result = Service.Delete{0}s(new[] {{ model }});", model.Code);
        sb.AppendLine("            UI.Result(result, () => grid.QueryData());");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private void ShowForm(string action, {0} model)", model.EntityName);
        sb.AppendLine("    {");
        sb.AppendLine("        UI.Show<{0}Form>($\"{{action}}{1}\", 800, 500, model, () => grid.QueryData());", model.Code, model.Name);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetFormRazorCode(DomainInfo model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@namespace Known.{0}.Pages", model.Project);
        sb.AppendLine("@inherits BaseForm");
        sb.AppendLine(" ");
        sb.AppendLine("<Form @ref=\"form\" IsTable=\"true\" Model=\"Model\">");
        sb.AppendLine("    <Hidden Id=\"Id\" />");
        sb.AppendLine("    @if (!IsDialog)");
        sb.AppendLine("    {");
        sb.AppendLine("        <div class=\"form-title\">{0}信息</div>", model.Name);
        sb.AppendLine("    }");
        sb.AppendLine("    <table style=\"@style\">");
        sb.AppendLine("        <colgroup>");
        sb.AppendLine("            <col style=\"width:120px\" />");
        sb.AppendLine("            <col />");
        sb.AppendLine("        </colgroup>");
        foreach (var item in model.Fields)
        {
            var required = item.Required ? " Required=\"true\"" : "";
            sb.AppendLine("        <tr><{0} Label=\"{1}\" Id=\"{2}\"{3} /></tr>", item.Type, item.Name, item.Code, required);
        }
        sb.AppendLine("    </table>");
        sb.AppendLine("</Form>");
        sb.AppendLine("<div class=\"form-button\">");
        sb.AppendLine("    <ButtonSave OnClick=\"OnSubmit\" />");
        sb.AppendLine("    @if (IsDialog)");
        sb.AppendLine("    {");
        sb.AppendLine("        <ButtonClose />");
        sb.AppendLine("    }");
        sb.AppendLine("</div>");
        sb.AppendLine(" ");
        sb.AppendLine("@code {");
        sb.AppendLine("    private Form form;");
        sb.AppendLine("    private string style;");
        sb.AppendLine(" ");
        sb.AppendLine("    protected override void OnInitialized()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnInitialized();");
        sb.AppendLine("        style = IsDialog ? \"\" : \"width:60%;margin:0 auto;\";");
        sb.AppendLine("        if (Model == null)");
        sb.AppendLine("        {");
        sb.AppendLine("            Model = new {0}", model.EntityName);
        sb.AppendLine("            {");
        sb.AppendLine("            };");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    private void OnSubmit()");
        sb.AppendLine("    {");
        sb.AppendLine("        Submit(form, data => Service.Save{0}(data), !IsDialog);", model.Code);
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

        return "string?";
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