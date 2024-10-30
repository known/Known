namespace Known.Core.Services;

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

class SysUserImport(ImportContext context) : ImportBase<SysUser>(context)
{
    public override void InitColumns()
    {
        AddColumn(c => c.UserName);
        AddColumn(c => c.Name);
        AddColumn(c => c.EnglishName);
        AddColumn(c => c.Gender);
        AddColumn(c => c.Phone);
        AddColumn(c => c.Mobile);
        AddColumn(c => c.Email);
        AddColumn(c => c.Note);
    }

    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var models = new List<SysUser>();
        var result = ImportHelper.ReadFile<SysUser>(Context, file, item =>
        {
            var model = new SysUser
            {
                UserName = item.GetValue(c => c.UserName),
                Name = item.GetValue(c => c.Name),
                EnglishName = item.GetValue(c => c.EnglishName),
                Gender = item.GetValue(c => c.Gender),
                Phone = item.GetValue(c => c.Phone),
                Mobile = item.GetValue(c => c.Mobile),
                Email = item.GetValue(c => c.Email),
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

        var database = Database;
        var info = await SystemService.GetSystemAsync(database);
        return await database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                item.OrgNo = db.User.CompNo;
                item.Password = CoreUtils.ToMd5(info?.UserDefaultPwd);
                await db.SaveAsync(item);
            }
        });
    }
}