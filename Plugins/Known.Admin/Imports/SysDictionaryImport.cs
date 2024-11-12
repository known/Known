namespace Known.Admin.Imports;

class SysDictionaryImport(ImportContext context) : ImportBase<SysDictionary>(context)
{
    public override void InitColumns()
    {
        AddColumn(c => c.Category);
        AddColumn(c => c.Code);
        AddColumn(c => c.Name);
        AddColumn(c => c.Sort);
        AddColumn(c => c.Note);
    }

    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var models = new List<SysDictionary>();
        var result = ImportHelper.ReadFile<SysDictionary>(Context, file, item =>
        {
            var model = new SysDictionary
            {
                Category = item.GetValue(c => c.Category),
                CategoryName = item.GetValue(c => c.Category),
                Code = item.GetValue(c => c.Code),
                Name = item.GetValue(c => c.Name),
                Sort = item.GetValueT(c => c.Sort),
                Note = item.GetValue(c => c.Note),
                Enabled = true
            };
            var vr = model.Validate(Context);
            if (!vr.IsValid)
                item.ErrorMessage = vr.Message;
            else
                models.Add(model);
        });

        if (!result.IsValid)
            return result;

        return await Database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(item);
            }
        });
    }
}