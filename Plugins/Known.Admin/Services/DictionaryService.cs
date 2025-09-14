namespace Known.Services;

/// <summary>
/// 数据字典服务接口。
/// </summary>
public interface IDictionaryService : IService
{
    /// <summary>
    /// 异步分页查询数据字典。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取数据字典类别列表。
    /// </summary>
    /// <returns>数据字典类别列表。</returns>
    Task<List<CodeInfo>> GetCategoriesAsync();

    /// <summary>
    /// 异步删除数据字典。
    /// </summary>
    /// <param name="infos">数据字典列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos);

    /// <summary>
    /// 异步保存数据字典。
    /// </summary>
    /// <param name="info">数据字典对象。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveDictionaryAsync(UploadInfo<DictionaryInfo> info);
}

[Client]
class DictionaryClient(HttpClient http) : ClientBase(http), IDictionaryService
{
    public Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<DictionaryInfo>("/Dictionary/QueryDictionaries", criteria);
    }

    public Task<List<CodeInfo>> GetCategoriesAsync()
    {
        return Http.GetAsync<List<CodeInfo>>("/Dictionary/GetCategories");
    }

    public Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        return Http.PostAsync("/Dictionary/DeleteDictionaries", infos);
    }

    public Task<Result> SaveDictionaryAsync(UploadInfo<DictionaryInfo> info)
    {
        return Http.PostAsync("/Dictionary/SaveDictionary", info);
    }
}

[WebApi, Service]
class DictionaryService(Context context) : ServiceBase(context), IDictionaryService
{
    public async Task<Result> RefreshCacheAsync()
    {
        var codes = await Database.GetDictionariesAsync();
        Cache.AttachCodes(codes);
        return Result.Success(AdminLanguage.RefreshSuccess, codes);
    }

    public async Task<List<CodeInfo>> GetCategoriesAsync()
    {
        var categories = await Database.Query<SysDictionary>()
                                       .Where(d => d.Enabled && d.Category == Constants.DicCategory)
                                       .OrderBy(d => d.Sort)
                                       .ToListAsync();
        return categories?.Select(c => new CodeInfo(c.Category, c.Code, c.Name, c.CategoryName)).ToList();
    }

    public async Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        List<SysDictionary> categories = [];
        PagingResult<DictionaryInfo> result = null;
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [nameof(DictionaryInfo.Sort)];
        await Database.QueryActionAsync(async db =>
        {
            categories = await db.QueryListAsync<SysDictionary>(d => d.Category == Constants.DicCategory);
            result = await db.Query<SysDictionary>(criteria).ToPageAsync<DictionaryInfo>();
        });
        if (result.TotalCount > 0)
        {
            foreach (var item in result.PageData)
            {
                var category = categories?.FirstOrDefault(d => d.Code == item.Category);
                item.DicType = Utils.ConvertTo<DictionaryType>(category?.CategoryName);
            }
        }
        return result;
    }

    public async Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var item in infos)
        {
            if (await database.ExistsAsync<SysDictionary>(d => d.Category == item.Code))
                return Result.Error(AdminLanguage.TipDicDeleteExistsChild);
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

    public async Task<Result> SaveDictionaryAsync(UploadInfo<DictionaryInfo> info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysDictionary>(info.Model.Id);
        model ??= new SysDictionary();
        model.FillModel(info.Model);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var exists = await database.ExistsAsync<SysDictionary>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Category == model.Category && d.Code == model.Code);
        if (exists)
            return Result.Error(AdminLanguage.TipDicCodeExists);

        var fileFiles = info.Files?.GetAttachFiles(nameof(DictionaryInfo.Extension), "DictionaryFiles");
        var result = await database.TransactionAsync(Language.Save, async db =>
        {
            await db.AddFilesAsync(fileFiles, model.Id, key => model.Extension = key);
            await db.SaveAsync(model);
            info.Model.Id = model.Id;
        }, info.Model);
        if (result.IsValid)
            await RefreshCacheAsync();
        return result;
    }
}