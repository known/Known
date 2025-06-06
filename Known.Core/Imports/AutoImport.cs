﻿namespace Known.Imports;

class AutoImport(ImportContext context) : ImportBase(context)
{
    public override async Task<Result> ExecuteAsync(AttachInfo file)
    {
        var database = Database;
        var param = await database.GetAutoPageAsync(ImportContext.PageId, ImportContext.PluginId);
        if (param == null)
            return Result.Error(Language.Required(CoreLanguage.EntityPlugin));

        var tableName = param.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required(CoreLanguage.TableName));

        var fields = param.Form?.Fields;
        if (fields == null)
            return Result.Error(Language.Required(CoreLanguage.Fields));

        var models = new List<Dictionary<string, object>>();
        var result = ImportHelper.ReadFile<Dictionary<string, object>>(Context, file, item =>
        {
            var model = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                if (field.Type == FieldType.Date || field.Type == FieldType.DateTime)
                    model[field.Id] = item.GetValue<DateTime?>(field.Name);
                else
                    model[field.Id] = item.GetValue(field.Name);
            }
            models.Add(model);
        });

        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(tableName, item);
            }
        });
    }
}