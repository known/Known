namespace Known.Imports;

class UserInfoImport(ImportContext context) : ImportBase<SysUser>(context)
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

    public override async Task<Result> ExecuteAsync(AttachInfo file)
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
        var info = await database.GetSystemAsync();
        return await database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                item.OrgNo = db.User.CompNo;
                item.Password = Utils.ToMd5(info?.UserDefaultPwd);
                await db.SaveAsync(item);
            }
        });
    }
}