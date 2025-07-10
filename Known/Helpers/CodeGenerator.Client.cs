namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetClient(PageInfo page, EntityInfo entity)
    {
        var modelName = GetModelName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var pluralName = GetPluralName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", Model.Namespace);
        AppendClient(sb, modelName, className, pluralName, page);
        return sb.ToString().TrimEnd([.. Environment.NewLine]);
    }

    private void AppendClient(StringBuilder sb, string modelName, string className, string pluralName, PageInfo page)
    {
        sb.AppendLine(" ");
        sb.AppendLine("[Client]");
        sb.AppendLine("class {0}Client(HttpClient http) : ClientBase(http), I{0}Service", className);
        sb.AppendLine("{");
        sb.AppendLine("    public Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria)", modelName, pluralName);
        sb.AppendLine("    {");
        sb.AppendLine("        return Http.QueryAsync<{0}>(\"/{1}/Query{2}\", criteria);", modelName, className, pluralName);
        sb.AppendLine("    }");

        if (HasSave(page))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public Task<{0}> Get{1}Async(string id)", modelName, className);
            sb.AppendLine("    {");
            sb.AppendLine("        return Http.GetAsync<{1}>($\"/{0}/Get{2}?id={{id}}\");", className, modelName, className);
            sb.AppendLine("    }");
        }

        if (HasDelete(page))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public Task<Result> Delete{0}Async(List<{1}> infos)", pluralName, modelName);
            sb.AppendLine("    {");
            sb.AppendLine("        return Http.PostAsync(\"/{0}/Delete{1}\", infos);", className, pluralName);
            sb.AppendLine("    }");
        }

        if (page.Tools != null && page.Tools.Count > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "DeleteM" || item == "Import" || item == "Export")
                    continue;

                sb.AppendLine(" ");
                sb.AppendLine("    public Task<Result> {0}{1}Async(List<{2}> infos)", item, pluralName, modelName);
                sb.AppendLine("    {");
                sb.AppendLine("        return Http.PostAsync(\"/{0}/{1}{2}\", infos);", className, item, pluralName);
                sb.AppendLine("    }");
            }
        }

        if (page.Actions != null && page.Actions.Count > 0)
        {
            foreach (var item in page.Actions)
            {
                if (item == "Edit" || item == "Delete")
                    continue;

                sb.AppendLine(" ");
                sb.AppendLine("    public Task<Result> {0}{1}Async({2} info)", item, className, modelName);
                sb.AppendLine("    {");
                sb.AppendLine("        return Http.PostAsync(\"/{0}/{1}{0}\", info);", className, item);
                sb.AppendLine("    }");
            }
        }

        if (HasSave(page))
        {
            var modelClass = Model.HasFile ? $"UploadInfo<{modelName}>" : modelName;
            sb.AppendLine(" ");
            sb.AppendLine("    public Task<Result> Save{0}Async({1} info)", className, modelClass);
            sb.AppendLine("    {");
            sb.AppendLine("        return Http.PostAsync(\"/{0}/Save{0}\", info);", className);
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
    }
}