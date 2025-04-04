namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetService(PageInfo page, EntityInfo entity)
    {
        var modelName = entity.ModelName ?? entity.Id;
        var entityName = entity.EntityName ?? entity.Id;
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", entity.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("[WebApi, Service]");
        sb.AppendLine("class {0}Service(Context context) : ServiceBase(context), I{0}Service", className);
        sb.AppendLine("{");
        sb.AppendLine("    public Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria)", modelName, pluralName);
        sb.AppendLine("    {");
        sb.AppendLine("        return Database.Query<{0}>(criteria).ToPageAsync<{1}>();", entityName, modelName);
        sb.AppendLine("    }");

        if (HasDelete(page))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task<Result> Delete{0}Async(List<{1}> infos)", pluralName, modelName);
            sb.AppendLine("    {");
            sb.AppendLine("        if (infos == null || infos.Count == 0)");
            sb.AppendLine("            return Result.Error(Language.SelectOneAtLeast);");
            sb.AppendLine(" ");
            sb.AppendLine("        return await Database.TransactionAsync(Language.Delete, async db =>");
            sb.AppendLine("        {");
            sb.AppendLine("            foreach (var item in infos)");
            sb.AppendLine("            {");
            sb.AppendLine("                await db.DeleteAsync<{0}>(item.Id);", entityName);
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            sb.AppendLine("    }");
        }

        if (page.Tools != null && page.Tools.Count > 0)
        {
            foreach (var item in page.Tools)
            {
                if (item == "New" || item == "DeleteM" || item == "Import" || item == "Export")
                    continue;

                sb.AppendLine(" ");
                sb.AppendLine("    public async Task<Result> {0}{1}Async(List<{2}> infos)", item, pluralName, modelName);
                sb.AppendLine("    {");
                sb.AppendLine("        if (infos == null || infos.Count == 0)");
                sb.AppendLine("            return Result.Error(Language.SelectOneAtLeast);");
                sb.AppendLine(" ");
                sb.AppendLine("        return await Database.TransactionAsync(Language[\"Button.{0}\"], async db =>", item);
                sb.AppendLine("        {");
                sb.AppendLine("        });");
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
                sb.AppendLine("    public async Task<Result> {0}{1}Async({2} info)", item, className, modelName);
                sb.AppendLine("    {");
                sb.AppendLine("        throw new NotImplementedException();");
                sb.AppendLine("    }");
            }
        }

        if (HasSave(page))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task<Result> Save{0}Async({1} info)", className, modelName);
            sb.AppendLine("    {");
            sb.AppendLine("        var database = Database;");
            sb.AppendLine("        var model = await database.QueryByIdAsync<{0}>(info.Id);", entityName);
            sb.AppendLine("        model ??= new {0}();", entityName);
            sb.AppendLine("        model.FillModel(info);");
            sb.AppendLine(" ");
            sb.AppendLine("        var vr = model.Validate(Context);");
            sb.AppendLine("        if (!vr.IsValid)");
            sb.AppendLine("            return vr;");
            sb.AppendLine(" ");
            sb.AppendLine("        return await database.TransactionAsync(Language.Save, async db =>");
            sb.AppendLine("        {");
            sb.AppendLine("            await db.SaveAsync(model);");
            sb.AppendLine("            info.Id = model.Id;");
            sb.AppendLine("        }, info);");
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}