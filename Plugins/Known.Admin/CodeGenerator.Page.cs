﻿namespace Known;

partial class CodeGenerator
{
    public string GetPage(PageInfo page, EntityInfo entity)
    {
        var pluralName = GetPluralName(entity.Id);
        var className = AdminHelper.GetClassName(entity.Id);
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
        if (page.Tools != null && page.Tools.Count > 0)
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

        if (page.Actions != null && page.Actions.Count > 0)
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

        var className = AdminHelper.GetClassName(name);
        if (!className.EndsWith('y'))
            return className + "s";

        return className.Substring(0, className.Length - 1) + "ies";
    }
}