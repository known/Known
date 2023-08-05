namespace Sample.Core.Imports;

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

    public override Result Execute(SysFile file)
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

        return Database.Transaction("导入", db =>
        {
            foreach (var item in models)
            {
                db.Save(item);
            }
        });
    }
}