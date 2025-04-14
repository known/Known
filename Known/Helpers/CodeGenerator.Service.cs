namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetService(PageInfo page, EntityInfo entity)
    {
        var hasFile = Model.HasFile;
        var modelName = Model.ModelName;
        if (string.IsNullOrWhiteSpace(modelName))
            modelName = entity.Id;

        var entityName = Model.EntityName;
        if (string.IsNullOrWhiteSpace(entityName))
            entityName = entity.Id;

        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", Model.Namespace);
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
            sb.AppendLine("        var database = Database;");
            if (hasFile)
                sb.AppendLine("        var oldFiles = new List<string>();");
            sb.AppendLine("        var result = await database.TransactionAsync(Language.Delete, async db =>");
            sb.AppendLine("        {");
            sb.AppendLine("            foreach (var item in infos)");
            sb.AppendLine("            {");
            if (hasFile)
                sb.AppendLine("                await db.DeleteFilesAsync(item.Id, oldFiles);");
            sb.AppendLine("                await db.DeleteAsync<{0}>(item.Id);", entityName);
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            if (hasFile)
            {
                sb.AppendLine("        if (result.IsValid)");
                sb.AppendLine("            AttachFile.DeleteFiles(oldFiles);");
            }
            sb.AppendLine("        return result;");
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
                sb.AppendLine("        var database = Database;");
                sb.AppendLine("        return await database.TransactionAsync(Language[\"Button.{0}\"], async db =>", item);
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
            var modelClass = hasFile ? $"UploadInfo<{modelName}>" : modelName;
            var model = hasFile ? ".Model" : "";
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task<Result> Save{0}Async({1} info)", className, modelClass);
            sb.AppendLine("    {");
            sb.AppendLine("        var database = Database;");
            sb.AppendLine("        var model = await database.QueryByIdAsync<{0}>(info{1}.Id);", entityName, model);
            sb.AppendLine("        model ??= new {0}();", entityName);
            sb.AppendLine("        model.FillModel(info{0});", model);
            sb.AppendLine(" ");
            sb.AppendLine("        var vr = model.Validate(Context);");
            sb.AppendLine("        if (!vr.IsValid)");
            sb.AppendLine("            return vr;");
            sb.AppendLine(" ");
            if (hasFile)
                sb.AppendLine("        var bizFiles = info.Files.GetAttachFiles(nameof({0}.Files), \"{0}Files\");", modelName);
            sb.AppendLine("        return await database.TransactionAsync(Language.Save, async db =>");
            sb.AppendLine("        {");
            if (hasFile)
                sb.AppendLine("            await db.AddFilesAsync(bizFiles, model.Id, key => model.Files = key);");
            sb.AppendLine("            await db.SaveAsync(model);");
            sb.AppendLine("            info{0}.Id = model.Id;", model);
            sb.AppendLine("        }}, info{0});", model);
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}