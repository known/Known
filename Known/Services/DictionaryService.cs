namespace Known.Services;

public interface IDictionaryService : IService
{
    Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria);
    Task<List<CodeInfo>> GetCategoriesAsync();
    Task<Result> DeleteDictionariesAsync(List<SysDictionary> models);
    Task<Result> SaveDictionaryAsync(SysDictionary model);
}

class DictionaryClient(HttpClient http) : ClientBase(http), IDictionaryService
{
    public Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria) => QueryAsync<SysDictionary>("Dictionary/QueryDictionaries", criteria);
    public Task<List<CodeInfo>> GetCategoriesAsync() => GetAsync<List<CodeInfo>>("Dictionary/GetCategories");
    public Task<Result> DeleteDictionariesAsync(List<SysDictionary> models) => PostAsync("Dictionary/DeleteDictionaries", models);
    public Task<Result> SaveDictionaryAsync(SysDictionary model) => PostAsync("Dictionary/SaveDictionary", model);
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
        var categories = await DictionaryRepository.GetCategoriesAsync(Database);
        return categories?.Select(c => new CodeInfo(c.Category, c.Code, c.Name)).ToList();
    }

    public Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [nameof(SysDictionary.Sort)];
        return DictionaryRepository.QueryDictionariesAsync(Database, criteria);
    }

    public async Task<Result> DeleteDictionariesAsync(List<SysDictionary> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await DictionaryRepository.DeleteDictionariesAsync(db, item.Code);
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

        var exists = await DictionaryRepository.ExistsDictionary(Database, model);
        if (exists)
            return Result.Error(Language["Tip.DicCodeExists"]);

        await Database.SaveAsync(model);
        await RefreshCacheAsync();
        return Result.Success(Language.Success(Language.Save), model);
    }

    internal static async Task<List<CodeInfo>> GetDictionariesAsync(Database db)
    {
        var entities = await DictionaryRepository.GetDictionariesAsync(db);
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            if (!string.IsNullOrWhiteSpace(e.Name))
                code = $"{code}-{e.Name}";
            return new CodeInfo(e.Category, code, code, e);
        }).ToList();
        return codes;
    }
}