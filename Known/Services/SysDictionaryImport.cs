using Known.Entities;
using Known.Helpers;

namespace Known.Services;

class SysDictionaryImport : BaseImport
{
    public SysDictionaryImport(Database database) : base(database) { }

    public override List<ImportColumn> Columns
    {
        get
        {
            return
            [
                new("类别", true),
                new("代码"),
                new("名称"),
                new("顺序"),
                new("备注")
            ];
        }
    }

    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var models = new List<SysDictionary>();
        var result = ImportHelper.ReadFile(file, item =>
        {
            var model = new SysDictionary
            {
                Category = item.GetValue("类别"),
                CategoryName = item.GetValue("类别"),
                Code = item.GetValue("代码"),
                Name = item.GetValue("名称"),
                Sort = item.GetValue<int>("顺序"),
                Note = item.GetValue("备注"),
                Enabled = true
            };
            var vr = model.Validate();
            if (!vr.IsValid)
                item.ErrorMessage = vr.Message;
            else
                models.Add(model);
        });

        if (!result.IsValid)
            return result;

        return await Database.TransactionAsync("导入", async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(item);
            }
        });
    }
}