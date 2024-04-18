namespace Known.Services;

class DictionaryService(Context context) : ServiceBase(context)
{
    public Task<Result> RefreshCacheAsync() => RefreshCacheAsync(Database);

    public async Task<List<CodeInfo>> GetCategoriesAsync()
    {
        var categories = await DictionaryRepository.GetCategoriesAsync(Database);
        return categories?.Select(c => new CodeInfo(c.Category, c.Code, c.Name)).ToList();
    }

    public Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        return DictionaryRepository.QueryDictionarysAsync(Database, criteria);
    }

    public async Task<Result> DeleteDictionarysAsync(List<SysDictionary> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await DictionaryRepository.DeleteDictionarysAsync(db, item.Code);
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

    private async Task<Result> RefreshCacheAsync(Database db)
    {
        var entities = await DictionaryRepository.GetDictionarysAsync(db);
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            if (!string.IsNullOrWhiteSpace(e.Name))
                code = $"{code}-{e.Name}";
            return new CodeInfo(e.Category, code, code, e);
        }).ToList();
        Cache.AttachCodes(codes);
        return Result.Success(Language["Tip.RefreshSuccess"], codes);
    }
}