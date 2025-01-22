namespace Known.Services;

public partial interface IAdminService
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
    Task<Result> SaveDictionaryAsync(DictionaryInfo info);
}

partial class AdminService
{
    private const string KeyDictionary = "Dictionaries";

    public Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        var category = criteria.GetQueryValue(nameof(DictionaryInfo.Category));
        return QueryModelsAsync<DictionaryInfo>(KeyDictionary, criteria, datas =>
        {
            return datas?.Where(d => d.Category == category)
                         .OrderBy(d => d.Sort)
                         .ToList();
        });
    }

    public Task<List<CodeInfo>> GetCategoriesAsync()
    {
        var datas = AppData.GetBizData<List<DictionaryInfo>>(KeyDictionary);
        var codes = datas?.Where(c => c.Category == Constants.DicCategory)
                          .OrderBy(d => d.Sort)
                          .Select(c => new CodeInfo(c.Category, c.Code, c.Name))
                          .ToList();
        return Task.FromResult(codes);
    }

    public Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        return DeleteModelsAsync(KeyDictionary, infos);
    }

    public Task<Result> SaveDictionaryAsync(DictionaryInfo info)
    {
        return SaveModelAsync(KeyDictionary, info);
    }
}

partial class AdminClient
{
    public Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<DictionaryInfo>("/Admin/QueryDictionaries", criteria);
    }

    public Task<List<CodeInfo>> GetCategoriesAsync()
    {
        return Http.GetAsync<List<CodeInfo>>("/Admin/GetCategories");
    }

    public Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteDictionaries", infos);
    }

    public Task<Result> SaveDictionaryAsync(DictionaryInfo info)
    {
        return Http.PostAsync("/Admin/SaveDictionary", info);
    }
}