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
    public Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<DictionaryInfo>());
    }

    public Task<List<CodeInfo>> GetCategoriesAsync()
    {
        return Task.FromResult(new List<CodeInfo>());
    }

    public Task<Result> DeleteDictionariesAsync(List<DictionaryInfo> infos)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> SaveDictionaryAsync(DictionaryInfo info)
    {
        return Result.SuccessAsync("保存成功！");
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