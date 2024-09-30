namespace Known.Services;

/// <summary>
/// 系统数据字典服务接口。
/// </summary>
public interface IDictionaryService : IService
{
    /// <summary>
    /// 异步分页查询数据字典。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取数据字典类别列表。
    /// </summary>
    /// <returns>数据字典类别列表。</returns>
    Task<List<CodeInfo>> GetCategoriesAsync();

    /// <summary>
    /// 异步删除数据字典。
    /// </summary>
    /// <param name="models">数据字典列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteDictionariesAsync(List<SysDictionary> models);

    /// <summary>
    /// 异步保存数据字典。
    /// </summary>
    /// <param name="model">数据字典对象。</param>
    /// <returns>保存结果。</returns>
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
        var categories = await Repository.GetDicCategoriesAsync(Database);
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

    internal static async Task<List<CodeInfo>> GetDictionariesAsync(Database db)
    {
        var entities = await db.QueryListAsync<SysDictionary>();
        var codes = entities.Where(d => d.Enabled).OrderBy(d => d.Category).ThenBy(d => d.Sort).Select(e =>
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