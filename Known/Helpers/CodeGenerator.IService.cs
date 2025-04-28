namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetIService(PageInfo page, EntityInfo entity) => GetIService(page, entity, false);

    public string GetIService(PageInfo page, EntityInfo entity, bool hasClient)
    {
        var modelName = GetModelName(entity.Id);
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", Model.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("public interface I{0}Service : IService", className);
        sb.AppendLine("{");
        sb.AppendLine("    Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria);", modelName, pluralName);

        if (HasDelete(page))
            sb.AppendLine("    Task<Result> Delete{0}Async(List<{1}> infos);", pluralName, modelName);

        if (page.Tools != null && page.Tools.Count > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "DeleteM" || item == "Import" || item == "Export")
                    continue;

                sb.AppendLine("    Task<Result> {0}{1}Async(List<{2}> infos);", item, pluralName, modelName);
            }
        }

        if (page.Actions != null && page.Actions.Count > 0)
        {
            foreach (var item in page.Actions)
            {
                if (item == "Edit" || item == "Delete")
                    continue;

                sb.AppendLine("    Task<Result> {0}{1}Async({2} info);", item, className, modelName);
            }
        }

        if (HasSave(page))
        {
            var modelClass = Model.HasFile ? $"UploadInfo<{modelName}>" : modelName;
            sb.AppendLine("    Task<Result> Save{0}Async({1} info);", className, modelClass);
        }
        sb.AppendLine("}");

        if (hasClient)
            AppendClient(sb, modelName, className, pluralName, page);
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