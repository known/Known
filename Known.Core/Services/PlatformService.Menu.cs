namespace Known.Services;

partial class PlatformService
{
    public async Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        var database = Database;
        var module = await database.QueryByIdAsync<SysModule>(info.Id);
        if (module == null)
            return Result.Error("模块不存在！");

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            await db.DeleteAsync(module);
            var modules = await db.Query<SysModule>().Where(m => m.ParentId == info.ParentId).ToListAsync();
            if (modules != null && modules.Count > 0)
            {
                var index = 1;
                foreach (var item in modules)
                {
                    item.Sort = index++;
                    await db.SaveAsync(item);
                }
            }
        });
    }

    public async Task<Result> SaveMenuAsync(MenuInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysModule>(info.Id);
        model ??= new SysModule();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        if (string.IsNullOrWhiteSpace(model.Icon))
            model.Icon = "";//AntDesign不识别null值

        if (string.IsNullOrWhiteSpace(model.Code))
            model.Code = model.Name;
        model.LayoutData = Utils.ToJson(info.Layout);
        model.PluginData = info.Plugins?.ZipDataString();
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }
}