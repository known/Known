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
                                       .OrderBy(d => d.Sort)
                                       .ToListAsync();
        return categories?.Select(c => new CodeInfo(c.Category, c.Code, c.Name)).ToList();
    }

    public Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [nameof(DictionaryInfo.Sort)];
        return Database.Query<SysDictionary>(criteria).ToPageAsync<DictionaryInfo>();
    }

    public async Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var item in infos)
        {
            if (await database.ExistsAsync<DictionaryInfo>(d => d.Category == item.Code))
                return Result.Error(Language["Tip.DicDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysDictionary>(d => d.CompNo == db.User.CompNo && d.Category == item.Code);
                await db.DeleteAsync<SysDictionary>(item.Id);
            }
        });
    }

    public async Task<Result> SaveDictionaryAsync(DictionaryInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysDictionary>(info.Id);
        model ??= new SysDictionary();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var exists = await database.ExistsAsync<SysDictionary>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Category == model.Category && d.Code == model.Code);
        if (exists)
            return Result.Error(Language["Tip.DicCodeExists"]);

        await database.SaveAsync(model);
        await RefreshCacheAsync();
        info.Id = model.Id;
        return Result.Success(Language.SaveSuccess, info);
    }
}