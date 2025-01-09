namespace Known.Services;

partial class AdminService
{
    public async Task<Result> RefreshCacheAsync()
    {
        var codes = await Database.GetDictionariesAsync();
        Cache.AttachCodes(codes);
        return Result.Success(Language["Tip.RefreshSuccess"], codes);
    }

    public async Task<List<CodeInfo>> GetCategoriesAsync()
    {
        var categories = await Database.Query<SysDictionary>()
                                       .Where(d => d.Enabled && d.Category == Constants.DicCategory)
                                       .OrderBy(d => d.Sort).ToListAsync();
        return categories?.Select(c => new CodeInfo(c.Category, c.Code, c.Name)).ToList();
    }

    public Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [nameof(SysDictionary.Sort)];
        return Database.QueryPageAsync<SysDictionary>(criteria);
    }

    public async Task<Result> DeleteDictionariesAsync(List<SysDictionary> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in models)
        {
            if (await database.ExistsAsync<SysDictionary>(d => d.Category == model.Code))
                return Result.Error(Language["Tip.DicDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync<SysDictionary>(d => d.CompNo == db.User.CompNo && d.Category == item.Code);
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveDictionaryAsync(SysDictionary model)
    {
        model.CompNo = CurrentUser.CompNo;
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var database = Database;
        var exists = await database.ExistsAsync<SysDictionary>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Category == model.Category && d.Code == model.Code);
        if (exists)
            return Result.Error(Language["Tip.DicCodeExists"]);

        await database.SaveAsync(model);
        await RefreshCacheAsync();
        return Result.Success(Language.Success(Language.Save), model);
    }
}