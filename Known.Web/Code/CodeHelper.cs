using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.Web
{
    class CodeHelper
    {
        private readonly DomainInfo model;
        private List<FieldInfo> fields;

        public CodeHelper(DomainInfo model)
        {
            this.model = model;
            fields = GetFields(model.Fields);
        }

        private List<FieldInfo> GetFields(string field)
        {
            var lists = new List<FieldInfo>();
            if (string.IsNullOrEmpty(field))
                return lists;

            var fields = field.Split('\n');
            foreach (var item in fields)
            {
                //代码;Code;string;50;0;text
                var items = item.Split(';', '；');
                if (items.Length == 6)
                {
                    lists.Add(new FieldInfo
                    {
                        Name = items[0],
                        Code = items[1],
                        Type = items[2],
                        Length = items[3],
                        Required = items[4] == "1",
                        Control = items[5]
                    });
                }
            }

            return lists;
        }

        public string GetView()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class=\"layui-fit\">");
            sb.AppendLine("    <div class=\"fit-col\">");
            sb.AppendLine("        <table id=\"grid{0}\" class=\"layui-hide\" lay-filter=\"grid{0}\"></table>", model.Code);
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"form-card\" id=\"form{0}\">", model.Code);
            sb.AppendLine("    <div class=\"form-card-header\"></div>");
            sb.AppendLine("    <div class=\"form-card-body\">");
            sb.AppendLine("        <div class=\"form\">");
            sb.AppendLine("            <input type=\"hidden\" name=\"Id\">");
            foreach (var item in fields.Where(f => f.Control == "hidden"))
            {
                sb.AppendLine("            <input type=\"hidden\" name=\"{0}\">", item.Code);
            }
            var index = 0;
            foreach (var item in fields.Where(f => f.Control != "hidden"))
            {
                if (string.IsNullOrWhiteSpace(item.Control))
                    continue;

                var first = index++ == 0 ? " form-first-item" : "";
                var required = item.Required ? " required" : "";
                var block = item.Control == "textarea" ? "block" : "inline";
                sb.AppendLine("            <div class=\"layui-form-item{0}\">", first);
                sb.AppendLine("                <label class=\"layui-form-label{0}\">{1}</label>", required, item.Name);
                sb.AppendLine("                <div class=\"layui-input-{0}\">", block);
                AppendControl(sb, item, required);
                sb.AppendLine("                </div>");
                sb.AppendLine("            </div>");
            }
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"form-card-footer\"></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("");
            sb.AppendLine("<script>");
            sb.AppendLine("    layui.use('frame', function () {");
            sb.AppendLine("        var url = {");
            sb.AppendLine("            Query{0}s: '/{0}/Query{0}s',", model.Code);
            sb.AppendLine("            Delete{0}s: '/{0}/Delete{0}s',", model.Code);
            sb.AppendLine("            Get{0}: '/{0}/Get{0}',", model.Code);
            sb.AppendLine("            Save{0}: '/{0}/Save{0}'", model.Code);
            sb.AppendLine("        };");
            sb.AppendLine("");
            sb.AppendLine("        var $ = layui.jquery,");
            sb.AppendLine("            layer = layui.layer,");
            sb.AppendLine("            laydate = layui.laydate,");
            sb.AppendLine("            frame = layui.frame;");
            sb.AppendLine("");
            foreach (var item in fields.Where(f => f.Control == "date"))
            {
                sb.AppendLine("        laydate.render({{ elem: '#{0}' }});", item.Code);
            }
            sb.AppendLine("        var form = frame.form({");
            sb.AppendLine("            name: 'form{0}',", model.Code);
            sb.AppendLine("            defData: { Id: '' },");
            sb.AppendLine("            toolbar: [{");
            sb.AppendLine("                text: '保存', handler: function (e) {");
            sb.AppendLine("                    e.save(url.Save{0}, function () {{", model.Code);
            sb.AppendLine("                        grid.reload();");
            sb.AppendLine("                    });");
            sb.AppendLine("                }");
            sb.AppendLine("            }]");
            sb.AppendLine("        });");
            sb.AppendLine("");
            sb.AppendLine("        var grid = frame.grid({");
            sb.AppendLine("            name: 'grid{0}',", model.Code);
            sb.AppendLine("            config: {");
            sb.AppendLine("                url: url.Query{0}s,", model.Code);
            sb.AppendLine("                cols: [[");
            sb.AppendLine("                    { type: 'numbers', fixed: 'left' },");
            sb.AppendLine("                    { type: 'checkbox', fixed: 'left' },");
            index = 0;
            foreach (var item in fields)
            {
                var comma = ++index == fields.Count ? "" : ",";
                sb.AppendLine("                    {{ sort: true, title: '{0}', field: '{1}' }}{2}", item.Name, item.Code, comma);
            }
            sb.AppendLine("                ]]");
            sb.AppendLine("            },");
            sb.AppendLine("            toolbar: {");
            sb.AppendLine("                add: function (e) { form.show(); },");
            sb.AppendLine("                edit: function (e) { e.editRow(form); },");
            sb.AppendLine("                remove: function (e) {{ e.deleteRows(url.Delete{0}s); }}", model.Code);
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            sb.AppendLine("    });");
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        private void AppendControl(StringBuilder sb, FieldInfo item, string required)
        {
            switch (item.Control)
            {
                case "text":
                    sb.AppendLine("                    <input type=\"text\" name=\"{0}\" placeholder=\"\" autocomplete=\"off\"{1}>", item.Code, required);
                    break;
                case "textarea":
                    sb.AppendLine("                    <textarea name=\"{0}\" placeholder=\"\"{1}></textarea>", item.Code, required);
                    break;
                case "radio":
                    sb.AppendLine("                    <label class=\"form-radio\">");
                    sb.AppendLine("                        <input type=\"radio\" name=\"{0}\" value=\"tab\"{1}>", item.Code, required);
                    sb.AppendLine("                        <span>名称</span>");
                    sb.AppendLine("                    </label>");
                    break;
                case "checkbox":
                    sb.AppendLine("                    <label class=\"form-radio\">");
                    sb.AppendLine("                        <input type=\"checkbox\" name=\"{0}\" value=\"1\"{1}>", item.Code, required);
                    sb.AppendLine("                        <span>启用</span>");
                    sb.AppendLine("                    </label>");
                    break;
                case "select":
                    sb.AppendLine("                    <select name=\"{0}\" style=\"width:188px;\" code=\"WorksType\"{1}></select>", item.Code, required);
                    break;
                case "date":
                    sb.AppendLine("                    <input type=\"text\" id=\"{0}\" name=\"{0}\" placeholder=\"yyyy-MM-dd\" autocomplete=\"off\"{1}>", item.Code, required);
                    break;
                default:
                    break;
            }
            
        }

        public string GetEntity()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("");
            sb.AppendLine("namespace Known.Web.Entities");
            sb.AppendLine("{");
            sb.AppendLine("    public class {0} : BaseEntity", model.Code);
            sb.AppendLine("    {");

            var index = 0;
            foreach (var item in fields)
            {
                if (index++ > 0)
                    sb.AppendLine("");

                var tf = item.Required ? "true" : "false";
                var len = !string.IsNullOrWhiteSpace(item.Length) && !item.Length.Contains(",")
                        ? $", \"1\", \"{item.Length}\"" : "";
                var type = item.Type == "date" ? "DateTime" : item.Type;
                sb.AppendLine("        [Column(\"{0}\", \"\", {1}{2})]", item.Name, tf, len);
                sb.AppendLine("        public {0} {1} {{ get; set; }}", type, item.Code);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public string GetController()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web.Mvc;");
            sb.AppendLine("using Known.Web.Services;");
            sb.AppendLine("");
            sb.AppendLine("namespace Known.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    public class {0}Controller : BaseController", model.Code);
            sb.AppendLine("    {");
            sb.AppendLine("        private {0}Service Service => new {0}Service();", model.Code);
            sb.AppendLine("");
            sb.AppendLine("        #region View");
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine("        public ActionResult Query{0}s(CriteriaData data)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            return QueryPagingData(data, c => Service.Query{0}s(c));", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine("        public ActionResult Delete{0}s(string data)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            return PostAction<string[]>(data, d => Service.Delete{0}s(d));", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            sb.AppendLine("");
            sb.AppendLine("        #region Form");
            sb.AppendLine("        public ActionResult Get{0}(string id)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            return JsonResult(Service.Get{0}(id));", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("");
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

        public string GetService()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Known.Web.Entities;");
            sb.AppendLine("using Known.Web.Repositories;");
            sb.AppendLine("");
            sb.AppendLine("namespace Known.Web.Services");
            sb.AppendLine("{");
            sb.AppendLine("    class {0}Service : BaseService", model.Code);
            sb.AppendLine("    {");
            sb.AppendLine("        private I{0}Repository Repository => Container.Resolve<I{0}Repository>();", model.Code);
            sb.AppendLine("");
            sb.AppendLine("        #region View");
            sb.AppendLine("        public PagingResult<{0}> Query{0}s(PagingCriteria criteria)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            return Repository.Query{0}s(Database, criteria);", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public Result Delete{0}s(string[] ids)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            var entities = Database.QueryListById<{0}>(ids);", model.Code);
            sb.AppendLine("            if (entities == null || entities.Count == 0)");
            sb.AppendLine("                return Result.Error(\"请至少选择一条记录进行操作！\");");
            sb.AppendLine("");
            sb.AppendLine("            return Database.Transaction(\"删除\", db =>");
            sb.AppendLine("            {");
            sb.AppendLine("                foreach (var item in entities)");
            sb.AppendLine("                {");
            sb.AppendLine("                    db.Delete(item);");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            sb.AppendLine("");
            sb.AppendLine("        #region Form");
            sb.AppendLine("        public {0} Get{0}(string id)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            return Database.QueryById<{0}>(id);", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public Result Save{0}(dynamic model)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            var entity = Database.QueryById<{0}>((string)model.Id);", model.Code);
            sb.AppendLine("            if (entity == null)");
            sb.AppendLine("                entity = new {0} {{ CompNo = CurrentUser.CompNo }};", model.Code);
            sb.AppendLine("");
            sb.AppendLine("            entity.FillModel(model);");
            sb.AppendLine("            var vr = entity.Validate();");
            sb.AppendLine("            if (!vr.IsValid)");
            sb.AppendLine("                return vr;");
            sb.AppendLine("");
            sb.AppendLine("            Database.Save(entity);");
            sb.AppendLine("            return Result.Success(\"保存成功！\", entity.Id);");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public string GetRepository()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Known.Web.Entities;");
            sb.AppendLine("");
            sb.AppendLine("namespace Known.Web.Repositories");
            sb.AppendLine("{");
            sb.AppendLine("    public interface I{0}Repository", model.Code);
            sb.AppendLine("    {");
            sb.AppendLine("        PagingResult<{0}> Query{0}s(Database db, PagingCriteria criteria);", model.Code);
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    class {0}Repository : I{0}Repository", model.Code);
            sb.AppendLine("    {");
            sb.AppendLine("        public PagingResult<{0}> Query{0}s(Database db, PagingCriteria criteria)", model.Code);
            sb.AppendLine("        {");
            sb.AppendLine("            var sql = \"select * from {0} where CompNo=@CompNo\";", model.Code);
            sb.AppendLine("            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))");
            sb.AppendLine("            {");
            sb.AppendLine("                var key = criteria.Parameter.key;");
            sb.AppendLine("                sql += \" and Name like @key\";");
            sb.AppendLine("                criteria.Parameter.key = $\"%{key}%\";");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            return db.QueryPage<{0}>(sql, criteria);", model.Code);
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public string GetSql()
        {
            var sb = new StringBuilder();
            sb.AppendLine("--MySql");
            sb.AppendLine("create table `{0}` (", model.Code);
            sb.AppendLine("    `Id`         varchar(50) not null,");
            sb.AppendLine("    `CreateBy`   varchar(50) not null,");
            sb.AppendLine("    `CreateTime` datetime    not null,");
            sb.AppendLine("    `ModifyBy`   varchar(50) null,");
            sb.AppendLine("    `ModifyTime` datetime    null,");
            sb.AppendLine("    `Version`    int         not null,");
            sb.AppendLine("    `Extension`  text        null,");
            sb.AppendLine("    `CompNo`     varchar(50) not null,");
            foreach (var item in fields)
            {
                var required = item.Required ? "not null" : "null";
                var type = GetMySqlDbType(item);
                sb.AppendLine($"    `{item.Code}` {type} {required},");
            }
            sb.AppendLine("    PRIMARY KEY(`Id`)");
            sb.AppendLine(");");
            sb.AppendLine("");

            sb.AppendLine("--Oracle");
            sb.AppendLine("create table {0}(", model.Code);
            sb.AppendLine("    Id         varchar2(50)   not null,");
            sb.AppendLine("    CreateBy   varchar2(50)   not null,");
            sb.AppendLine("    CreateTime date           not null,");
            sb.AppendLine("    ModifyBy   varchar2(50)   null,");
            sb.AppendLine("    ModifyTime date           null,");
            sb.AppendLine("    Version    int            not null,");
            sb.AppendLine("    Extension  varchar2(4000) null,");
            sb.AppendLine("    CompNo     varchar2(50)   not null,");
            var index = 0;
            foreach (var item in fields)
            {
                var comma = ++index == fields.Count ? "" : ",";
                var required = item.Required ? "not null" : "null";
                var type = GetOracleDbType(item);
                sb.AppendLine($"    {item.Code} {type} {required}{comma}");
            }
            sb.AppendLine(");");
            sb.AppendLine("alter table {0} add constraint PK_{0} primary key(Id);", model.Code);
            
            sb.AppendLine("");
            sb.AppendLine("--SqlServer");
            sb.AppendLine("CREATE TABLE [{0}] (", model.Code);
            sb.AppendLine("    [Id]         [varchar](50)  NOT NULL,");
            sb.AppendLine("    [CreateBy]   [nvarchar](50) NOT NULL,");
            sb.AppendLine("    [CreateTime] [datetime]     NOT NULL,");
            sb.AppendLine("    [ModifyBy]   [nvarchar](50) NULL,");
            sb.AppendLine("    [ModifyTime] [datetime]     NULL,");
            sb.AppendLine("    [Version]    [int]          NOT NULL,");
            sb.AppendLine("    [Extension]  [ntext]        NULL,");
            sb.AppendLine("    [CompNo]     [varchar](50)  NOT NULL,");
            foreach (var item in fields)
            {
                var required = item.Required ? "NOT NULL" : "NULL";
                var type = GetSqlServerDbType(item);
                sb.AppendLine($"    [{item.Code}] {type} {required},");
            }
            sb.AppendLine("    CONSTRAINT [PK_{0}] PRIMARY KEY ([Id] ASC)", model.Code);
            sb.AppendLine(") ");
            sb.AppendLine("GO");

            return sb.ToString();
        }

        private string GetMySqlDbType(FieldInfo item)
        {
            var type = item.Type;
            if (type == "string")
                type = string.IsNullOrWhiteSpace(item.Length) ? "text" : "varchar";
            else if (type == "date")
                type = "datetime";

            if (!string.IsNullOrWhiteSpace(item.Length))
                type += $"({item.Length})";

            return type;
        }

        private string GetOracleDbType(FieldInfo item)
        {
            var type = item.Type;
            if (type == "string")
                type = string.IsNullOrWhiteSpace(item.Length) ? "varchar2(4000)" : "varchar2";

            if (!string.IsNullOrWhiteSpace(item.Length))
                type += $"({item.Length})";

            return type;
        }

        private string GetSqlServerDbType(FieldInfo item)
        {
            var type = item.Type;
            if (type == "string")
                type = string.IsNullOrWhiteSpace(item.Length) ? "ntext" : "nvarchar";
            else if (type == "date")
                type = "datetime";

            type = $"[{type}]";
            if (!string.IsNullOrWhiteSpace(item.Length))
                type += $"({item.Length})";

            return type;
        }
    }

    static class StringExtension
    {
        public static void AppendLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }
    }
}
