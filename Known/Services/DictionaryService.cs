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
    public Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        throw new NotImplementedException();
    }

    public Task<List<CodeInfo>> GetCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteDictionariesAsync(List<SysDictionary> models)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveDictionaryAsync(SysDictionary model)
    {
        throw new NotImplementedException();
    }
}