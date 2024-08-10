namespace Known.Services;

public interface IDictionaryService : IService
{
    Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria);
    Task<List<CodeInfo>> GetCategoriesAsync();
    Task<Result> DeleteDictionariesAsync(List<SysDictionary> models);
    Task<Result> SaveDictionaryAsync(SysDictionary model);
}

class DictionaryService(Context context) : ServiceBase(context), IDictionaryService
{
    public async Task<Result> RefreshCacheAsync()
    {
        var codes = await GetDictionariesAsync(Database);
        Cache.AttachCodes(codes);
        return Result.Success(Language["Tip.RefreshSuccess"], codes);
    }

    public async Task<List<CodeInfo>> GetCategoriesAsync()
    {
        var categories = await Database.Query<SysDictionary>()
                 .Where(d => d.Enabled && d.CompNo == CurrentUser.CompNo && d.Category == Constants.DicCategory)
                 .OrderBy(d => d.Sort)
                 .ToListAsync();
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

        return await Database.TransactionAsync(Language.Delete, async db =>
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

        var exists = await Database.ExistsAsync<SysDictionary>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Category == model.Category && d.Code == model.Code);
        if (exists)
            return Result.Error(Language["Tip.DicCodeExists"]);

        await Database.SaveAsync(model);
        await RefreshCacheAsync();
        return Result.Success(Language.Success(Language.Save), model);
    }

    internal static async Task<List<CodeInfo>> GetDictionariesAsync(Database db)
    {
        var entities = await db.Query<SysDictionary>()
                               .Where(d => d.Enabled)
                               .OrderBy(d => d.Category, d => d.Sort)
                               .ToListAsync();
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            var name = string.IsNullOrWhiteSpace(e.Name)
                     ? e.Code
                     : $"{e.Code}-{e.Name}";
            return new CodeInfo(e.Category, code, name, e);
        }).ToList();
        return codes;
    }
}