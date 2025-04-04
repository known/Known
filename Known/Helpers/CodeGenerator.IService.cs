namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetIService(PageInfo page, EntityInfo entity, bool hasClient = false)
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
        sb.AppendLine("    Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria);", entity.Id, pluralName);

        if (HasDelete(page))
            sb.AppendLine("    Task<Result> Delete{0}Async(List<{1}> infos);", pluralName, entity.Id);

        if (page.Tools != null && page.Tools.Count > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "DeleteM" || item == "Import" || item == "Export")
                    continue;

                sb.AppendLine("    Task<Result> {0}{1}Async(List<{2}> infos);", item, pluralName, entity.Id);
            }
        }

        if (page.Actions != null && page.Actions.Count > 0)
        {
            foreach (var item in page.Actions)
            {
                if (item == "Edit" || item == "Delete")
                    continue;

                sb.AppendLine("    Task<Result> {0}{1}Async({2} info);", item, className, entity.Id);
            }
        }

        if (HasSave(page))
            sb.AppendLine("    Task<Result> Save{0}Async({1} info);", className, entity.Id);
        sb.AppendLine("}");

        if (hasClient)
            AppendClient(sb, className, pluralName, page, entity);
        return sb.ToString();
    }

    private static bool HasSave(PageInfo page)
    {
        return page.Tools?.Contains("New") == true || page.Actions?.Contains("Edit") == true;
    }

    private static bool HasDelete(PageInfo page)
    {
        return page.Tools?.Contains("DeleteM") == true || page.Actions?.Contains("Delete") == true;
    }
}