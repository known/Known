/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Known.Dev.Models;

namespace Known.Dev.Services;

class CodeHelper
{
    private readonly DomainInfo model;
    private readonly List<FieldInfo> fields;
    private readonly string entityName;

    internal CodeHelper(DomainInfo model)
    {
        this.model = model;
        entityName = $"{model.Prefix}{model.Code}";
        fields = Utils.FromJson<List<FieldInfo>>(model.FieldData);
    }

    internal string GetView()
    {
        var prefix = model.Prefix.ToUpper();
        var controller = prefix == "SYS" ? "System" : $"{prefix}/{model.Code}";

        var sb = new StringBuilder();
        sb.AppendLine("function {0}{1}() {{", model.Prefix, model.Code);
        sb.AppendLine("    //fields");
        sb.AppendLine("    var url = {");
        sb.AppendLine("        QueryModels: baseUrl + '/{0}/Query{1}s',", controller, model.Code);
        sb.AppendLine("        DeleteModels: baseUrl + '/{0}/Delete{1}s',", controller, model.Code);
        sb.AppendLine("        //ImportModels: baseUrl + '/{0}/Import{1}s',", controller, model.Code);
        sb.AppendLine("        //GetModel: baseUrl + '/{0}/Get{1}',", controller, model.Code);
        sb.AppendLine("        SaveModel: baseUrl + '/{0}/Save{1}'", controller, model.Code);
        sb.AppendLine("    };");
        sb.AppendLine(" ");
        sb.AppendLine("    var view = new View('{0}', {{", model.Code);
        sb.AppendLine("        url: url,");
        sb.AppendLine("        columns: [");
        sb.AppendLine("            { field: 'Id', type: 'hidden' },");
        foreach (var item in fields)
        {
            item.SetDefault();
            var option = "";
            //if (!string.IsNullOrWhiteSpace(item.Align))
            //    option += $", align: '{item.Align}'";

            if (item.Query == 1)
                option += ", query: true";
            if (item.Sort == 1)
                option += ", sort: true";
            if (item.Import == 1)
                option += ", import: true";
            if (item.Export == 1)
                option += ", export: true";

            if (!string.IsNullOrWhiteSpace(item.Control))
                option += $", type: '{item.Control}'";
            if (item.Required == 1)
                option += ", required: true";

            if (!string.IsNullOrWhiteSpace(item.Codes))
            {
                var code = item.Codes.StartsWith("[") ? item.Codes : $"'{item.Codes}'";
                option += $", code: {code}";
            }

            if (item.Control == "date")
                sb.AppendLine("            {{ title: '{0}', field: '{1}', placeholder: 'yyyy-MM-dd HH:mm:ss'{2} }},", item.Name, item.Code, option);
            else if (item.Control == "textarea")
                sb.AppendLine("            {{ title: '{0}', field: '{1}'{2} }},", item.Name, item.Code, option);
            else
                sb.AppendLine("            {{ title: '{0}', field: '{1}'{2} }},", item.Name, item.Code, option);
        }
        sb.AppendLine("            { title: '修改人', field: 'ModifyBy', export: true },");
        sb.AppendLine("            { title: '修改时间', field: 'ModifyTime', placeholder: 'yyyy-MM-dd HH:mm:ss', export: true },");
        sb.AppendLine("            { title: '创建人', field: 'CreateBy', export: true },");
        sb.AppendLine("            { title: '创建时间', field: 'CreateTime', placeholder: 'yyyy-MM-dd HH:mm:ss', export: true }");
        sb.AppendLine("        ]");
        sb.AppendLine("    });");
        sb.AppendLine(" ");
        sb.AppendLine("    //methods");
        sb.AppendLine("    this.render = function(dom) {");
        sb.AppendLine("        view.render().appendTo(dom);");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    this.mounted = function() {");
        sb.AppendLine("        view.load();");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    //private");
        sb.AppendLine("}");

        return sb.ToString();
    }

    internal string GetEntity()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.Web.Entities");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// {0}实体类。", model.Name);
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public class {0} : EntityBase", entityName);
        sb.AppendLine("    {");

        var index = 0;
        foreach (var item in fields)
        {
            if (index++ > 0)
                sb.AppendLine(" ");

            var tf = item.Required == 1 ? "true" : "false";
            var len = !string.IsNullOrWhiteSpace(item.Length) && !item.Length.Contains(",")
                    ? $", \"1\", \"{item.Length}\"" : "";
            var type = item.Type == "date" ? "DateTime" : item.Type;
            if (item.Required != 1 && item.Type != "string")
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

    internal string GetController()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Web.Mvc;");
        sb.AppendLine("using Known.Web.Services;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.Web.Controllers");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// {0}控制器。", model.Name);
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public class {0}Controller : BaseController", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        private {0}Service Service => new {0}Service();", model.Code);
        sb.AppendLine(" ");
        sb.AppendLine("        #region View");
        sb.AppendLine("        [HttpPost]");
        sb.AppendLine("        public ActionResult Query{0}s(CriteriaData data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return QueryPagingData(data, c => Service.Query{0}s(c));", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        [HttpPost]");
        sb.AppendLine("        public ActionResult Delete{0}s(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return PostAction<string[]>(data, d => Service.Delete{0}s(d));", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        [HttpPost]");
        sb.AppendLine("        public ActionResult Import{0}s(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return ImportAction<{0}>(data, d => Service.Import{1}s(d));", entityName, model.Code);
        sb.AppendLine("        }");
        sb.AppendLine("        #endregion");
        sb.AppendLine(" ");
        sb.AppendLine("        #region Form");
        sb.AppendLine("        public ActionResult Get{0}(string id)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return JsonResult(Service.Get{0}(id));", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        [HttpPost]");
        sb.AppendLine("        public ActionResult Save{0}(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return PostAction<dynamic>(data, d => Service.Save{0}(d));", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine("        #endregion");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    internal string GetService()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Known.Web.Entities;");
        sb.AppendLine("using Known.Web.Repositories;");
        sb.AppendLine(" ");
        sb.AppendLine("namespace Known.Web.Services");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// {0}业务逻辑服务。", model.Name);
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public class {0}Service : BaseService", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        private I{0}Repository Repository => Container.Resolve<I{0}Repository>();", model.Code);
        sb.AppendLine(" ");
        sb.AppendLine("        #region View");
        sb.AppendLine("        public PagingResult<{0}> Query{1}s(PagingCriteria criteria)", entityName, model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return Repository.Query{0}s(Database, criteria);", model.Code);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        public Result Delete{0}s(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var ids = Utils.FromJson<string[]>(data);");
        sb.AppendLine("            var entities = Database.QueryListById<{0}>(ids);", entityName);
        sb.AppendLine("            if (entities == null || entities.Count == 0)");
        sb.AppendLine("                return Result.Error(\"请至少选择一条记录进行操作！\");");
        sb.AppendLine(" ");
        sb.AppendLine("            return Database.Transaction(\"删除\", db =>");
        sb.AppendLine("            {");
        sb.AppendLine("                foreach (var item in entities)");
        sb.AppendLine("                {");
        sb.AppendLine("                    db.Delete(item);");
        sb.AppendLine("                }");
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        public Result Import{0}s(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var entities = Utils.FromJson<List<{0}>>(data);", entityName);
        sb.AppendLine("            if (entities == null || entities.Count == 0)");
        sb.AppendLine("                return Result.Error(\"请至少导入一条记录！\");");
        sb.AppendLine(" ");
        sb.AppendLine("            return Database.Transaction(\"导入\", db =>");
        sb.AppendLine("            {");
        sb.AppendLine("                foreach (var item in entities)");
        sb.AppendLine("                {");
        sb.AppendLine("                    db.Insert(item);");
        sb.AppendLine("                }");
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine("        #endregion");
        sb.AppendLine(" ");
        sb.AppendLine("        #region Form");
        sb.AppendLine("        public {0} Get{1}(string id)", entityName, model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            return Database.QueryById<{0}>(id);", entityName);
        sb.AppendLine("        }");
        sb.AppendLine(" ");
        sb.AppendLine("        public Result Save{0}(string data)", model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var model = Utils.ToDynamic(data);");
        sb.AppendLine("            var entity = Database.QueryById<{0}>((string)model.Id);", entityName);
        sb.AppendLine("            if (entity == null)");
        sb.AppendLine("                entity = new {0} {{ CompNo = CurrentUser.CompNo }};", entityName);
        sb.AppendLine(" ");
        sb.AppendLine("            entity.FillModel(model);");
        sb.AppendLine("            var vr = entity.Validate();");
        sb.AppendLine("            if (!vr.IsValid)");
        sb.AppendLine("                return vr;");
        sb.AppendLine(" ");
        sb.AppendLine("            Database.Save(entity);");
        sb.AppendLine("            return Result.Success(\"保存成功！\", entity.Id);");
        sb.AppendLine("        }");
        sb.AppendLine("        #endregion");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    internal string GetRepository()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Known.Web.Entities;");
        sb.AppendLine("");
        sb.AppendLine("namespace Known.Web.Repositories");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// {0}数据依赖接口。", model.Name);
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public interface I{0}Repository", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        PagingResult<{0}> Query{1}s(Database db, PagingCriteria criteria);", entityName, model.Code);
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    class {0}Repository : I{0}Repository", model.Code);
        sb.AppendLine("    {");
        sb.AppendLine("        public PagingResult<{0}> Query{1}s(Database db, PagingCriteria criteria)", entityName, model.Code);
        sb.AppendLine("        {");
        sb.AppendLine("            var sql = \"select * from {0} where CompNo=@CompNo\";", entityName);
        foreach (var item in fields)
        {
            if (item.Query == 1)
            {
                sb.AppendLine("            db.SetQuery(ref sql, criteria, QueryType.Contain, \"{0}\");", item.Code);
            }
        }
        sb.AppendLine("            return db.QueryPage<{0}>(sql, criteria);", entityName);
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    internal string GetSql()
    {
        var columns = new List<FieldInfo>();
        columns.Add(new FieldInfo { Code = "Id", Type = "string", Length = "50", Required = 1 });
        columns.Add(new FieldInfo { Code = "CreateBy", Type = "string", Length = "50", Required = 1 });
        columns.Add(new FieldInfo { Code = "CreateTime", Type = "date", Required = 1 });
        columns.Add(new FieldInfo { Code = "ModifyBy", Type = "string", Length = "50" });
        columns.Add(new FieldInfo { Code = "ModifyTime", Type = "date" });
        columns.Add(new FieldInfo { Code = "Version", Type = "int", Required = 1 });
        columns.Add(new FieldInfo { Code = "Extension", Type = "string" });
        columns.Add(new FieldInfo { Code = "CompNo", Type = "string", Length = "50", Required = 1 });
        columns.AddRange(fields);

        var maxLength = columns.Select(f => (f.Code ?? "").Length).Max();
        var sb = new StringBuilder();
        sb.AppendLine("--MySql");
        sb.AppendLine("create table `{0}{1}` (", model.Prefix, model.Code);
        foreach (var item in columns)
        {
            var required = item.Required == 1 ? "not null" : "null";
            var column = $"`{item.Code}`";
            column = GetColumnName(column, maxLength + 2);
            var type = GetMySqlDbType(item);
            sb.AppendLine($"    {column} {type} {required},");
        }
        sb.AppendLine("    PRIMARY KEY(`Id`)");
        sb.AppendLine(");");
        sb.AppendLine("");

        sb.AppendLine("--Oracle");
        sb.AppendLine("create table {0}{1}(", model.Prefix, model.Code);
        var index = 0;
        foreach (var item in columns)
        {
            var comma = ++index == columns.Count ? "" : ",";
            var required = item.Required == 1 ? "not null" : "null";
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
            var required = item.Required == 1 ? "NOT NULL" : "NULL";
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

    private string GetColumnName(string column, int maxLength)
    {
        column = column ?? "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
    }

    private string GetMySqlDbType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "string")
            type = string.IsNullOrWhiteSpace(item.Length) ? "text" : "varchar";
        else if (type == "date")
            type = "datetime";

        if (type == "decimal" && string.IsNullOrWhiteSpace(item.Length))
            item.Length = "18,9";

        if (!string.IsNullOrWhiteSpace(item.Length))
            type += $"({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private string GetOracleDbType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "string")
            type = string.IsNullOrWhiteSpace(item.Length) ? "varchar2(4000)" : "varchar2";
        else if (type == "int")
            type = "number(8)";

        if (type == "decimal" && string.IsNullOrWhiteSpace(item.Length))
            item.Length = "18,9";

        if (!string.IsNullOrWhiteSpace(item.Length))
            type += $"({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }

    private string GetSqlServerDbType(FieldInfo item)
    {
        var type = item.Type ?? "";
        if (type == "string")
        {
            if (string.IsNullOrWhiteSpace(item.Length))
                type = "ntext";
            else if (item.Code.EndsWith("Id") || item.Code.EndsWith("No") || item.Code == "CompNo")
                type = "varchar";
            else
                type = "nvarchar";
        }
        else if (type == "date")
        {
            type = "datetime";
        }

        if (type == "decimal" && string.IsNullOrWhiteSpace(item.Length))
            item.Length = "18,9";

        type = $"[{type}]";
        if (!string.IsNullOrWhiteSpace(item.Length))
            type += $"({item.Length})";

        if (type.Length < 16)
            type += new string(' ', 16 - type.Length);

        return type;
    }
}

static class StringExtension
{
    internal static void AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        sb.AppendLine(string.Format(format, args));
    }
}