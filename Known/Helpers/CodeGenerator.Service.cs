namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetService(PageInfo page, EntityInfo entity)
    {
        var hasFile = Model.HasFile;
        var entityName = Model.EntityName;
        if (string.IsNullOrWhiteSpace(entityName))
            entityName = entity.Id;

        var modelName = GetModelName(entity.Id);
        var pluralName = GetPluralName(entity.Id);
        var className = DataHelper.GetClassName(entity.Id);
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Services;", Model.Namespace);
        sb.AppendLine(" ");
        sb.AppendLine("[WebApi, Service]");
        if (Model.IsAutoMode)
            sb.AppendLine("class {0}Service(Context context) : ServiceBase(context), I{0}Service", className);
        else
            sb.AppendLine("class {0}Service(Context context) : ServiceBase(context)", className);
        sb.AppendLine("{");
        sb.AppendLine("    public Task<PagingResult<{0}>> Query{1}Async(PagingCriteria criteria)", modelName, pluralName);
        sb.AppendLine("    {");
        if (Model.IsAutoMode)
            sb.AppendLine("        return Database.Query<{0}>(criteria).ToPageAsync<{1}>();", entityName, modelName);
        else
            sb.AppendLine("        return Database.QueryPageAsync<{0}>(criteria);", entityName);
        sb.AppendLine("    }");

        if (HasSave(page))
        {
            sb.AppendLine(" ");
            sb.AppendLine("    public async Task<{0}> Get{1}Async(string id)", modelName, className);
            sb.AppendLine("    {");
            sb.AppendLine("        var info = await  Database.Query<{0}>().FirstAsync<{1}>(d => d.Id == id);", entityName, modelName);
            sb.AppendLine("        info ??= new {0}();", modelName);
            sb.AppendLine("        return info;");
            sb.AppendLine("    }");
        }

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
                sb.AppendLine("        return await database.TransactionAsync(Language[\"{0}\"], async db =>", item);
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
            var fileFields = Model.Fields.Where(f => f.Type == FieldType.File).ToList() ?? [];
            foreach (var item in fileFields)
            {
                sb.AppendLine("        var file{0} = info.Files?.GetAttachFiles(nameof({1}.{0}), \"{2}Files\");", item.Id, modelName, className);
            }
            sb.AppendLine("        return await database.TransactionAsync(Language.Save, async db =>");
            sb.AppendLine("        {");
            foreach (var item in fileFields)
            {
                sb.AppendLine("            await db.AddFilesAsync(file{0}, model.Id, key => model.{0} = key);", item.Id);
            }
            sb.AppendLine("            await db.SaveAsync(model);");
            sb.AppendLine("            info{0}.Id = model.Id;", model);
            sb.AppendLine("        }}, info{0});", model);
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        return sb.ToString().TrimEnd([.. Environment.NewLine]);
    }
}