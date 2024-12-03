namespace Known.Core;

class DictionaryImport(ImportContext context) : ImportBase(context)
{
    public override async Task<Result> ExecuteAsync(AttachInfo file)
    {
        var module = DataHelper.GetModule(ImportContext.BizParam);
        if (module == null)
            return Result.Error(Language.Required("ModuleId"));

        var entity = DataHelper.ToEntity(module.EntityData);
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return Result.Error(Language.Required("TableName"));

        var fields = module.GetFormFields();
        if (fields == null)
            return Result.Error(Language.Required("Form.Fields"));

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

        return await Database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(entity.Id, item);
            }
        });
    }
}