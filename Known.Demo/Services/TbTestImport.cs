namespace Known.Demo.Services;

class TbTestImport : BaseImport
{
    public TbTestImport(Database database) : base(database) { }

    public override List<ImportColumn> Columns
    {
        get
        {
            return new List<ImportColumn>
            {
                new ImportColumn("备注")
            };
        }
    }

    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var models = new List<TbApply>();
        var result = ImportHelper.ReadFile(file, item =>
        {
            var model = new TbApply
            {
                //Note = item.GetValue("备注")
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