namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetPage(PageInfo page, EntityInfo entity)
    {
        var modelName = GetModelName(entity.Id);
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Pages;", Model.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("[Route(\"{0}\")]", entity.PageUrl);
        sb.AppendLine("[Menu(Constants.BaseData, \"{0}\", \"file\", 1)]", entity.Name);
        sb.AppendLine("public class {0}List : BaseTablePage<{1}>", className, modelName);
        sb.AppendLine("{");
        if (Model.IsAutoMode)
            sb.AppendLine("    private I{0}Service Service;", className);
        else
            sb.AppendLine("    private {0}Service Service => new {0}Service(Context);", className);
        sb.AppendLine(" ");
        sb.AppendLine("    protected override async Task OnInitPageAsync()");
        sb.AppendLine("    {");
        sb.AppendLine("        await base.OnInitPageAsync();");
        if (Model.IsAutoMode)
            sb.AppendLine("        Service = await CreateServiceAsync<I{0}Service>();", className);
        sb.AppendLine("        Table.FormType = typeof({0});", Model.FormName);
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
                    import = "    [Action] public Task Import() => Table.ShowImportAsync();";
                    continue;
                }

                if (item == "Export")
                {
                    export = "    [Action] public Task Export() => Table.ExportDataAsync();";
                    continue;
                }

                if (item == "New")
                    sb.AppendLine("    [Action] public void New() => Table.NewForm(Service.Save{0}Async, new {1}());", className, modelName);
                else if (item == "DeleteM")
                    sb.AppendLine("    [Action] public void DeleteM() => Table.DeleteM(Service.Delete{0}Async);", pluralName);
                else
                    sb.AppendLine("    [Action] public void {0}() => Table.SelectRows(Service.{0}{1}Async, Language[\"Button.{0}\"]);", item, pluralName);
            }
        }

        if (page.Actions != null && page.Actions.Count > 0)
        {
            foreach (var item in page.Actions)
            {
                if (item == "Edit")
                    sb.AppendLine("    [Action] public void Edit({0} row) => Table.EditForm(Service.Save{1}Async, row);", modelName, className);
                else if (item == "Delete")
                    sb.AppendLine("    [Action] public void Delete({0} row) => Table.Delete(Service.Delete{1}Async, row);", modelName, pluralName);
                else
                    sb.AppendLine("    [Action] public void {0}({1} row) => {{}};", item, modelName);
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
}