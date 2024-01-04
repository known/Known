using Known.Entities;
using Known.Helpers;

namespace Known.Services;

class SysDictionaryImport : ImportBase
{
    public SysDictionaryImport(Context context, Database database) : base(context, database) { }

    public override List<ImportColumn> Columns
    {
        get
        {
            return
            [
                new(Language[nameof(SysDictionary.Category)], true),
                new(Language[nameof(SysDictionary.Code)]),
                new(Language[nameof(SysDictionary.Name)]),
                new(Language[nameof(SysDictionary.Sort)]),
                new(Language[nameof(SysDictionary.Note)])
            ];
        }
    }

    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var models = new List<SysDictionary>();
        var result = ImportHelper.ReadFile(Context, file, item =>
        {
            var model = new SysDictionary
            {
                Category = item.GetValue(Language[nameof(SysDictionary.Category)]),
                CategoryName = item.GetValue(Language[nameof(SysDictionary.Category)]),
                Code = item.GetValue(Language[nameof(SysDictionary.Code)]),
                Name = item.GetValue(Language[nameof(SysDictionary.Name)]),
                Sort = item.GetValue<int>(Language[nameof(SysDictionary.Sort)]),
                Note = item.GetValue(Language[nameof(SysDictionary.Note)]),
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