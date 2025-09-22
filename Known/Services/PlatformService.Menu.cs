namespace Known.Services;

public partial interface IPlatformService
{
    /// <summary>
    /// 异步删除菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> DeleteMenuAsync(MenuInfo info);

    /// <summary>
    /// 异步保存菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveMenuAsync(MenuInfo info);
}

partial class PlatformClient
{
    public Task<Result> DeleteMenuAsync(MenuInfo info) => Http.PostAsync("/Platform/DeleteMenu", info);
    public Task<Result> SaveMenuAsync(MenuInfo info) => Http.PostAsync("/Platform/SaveMenu", info);
}

partial class PlatformService
{
    public async Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        var database = Database;
        var module = await database.QueryByIdAsync<SysModule>(info.Id);
        if (module == null)
            return Result.Error(Language.TipModuleNotExists);

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