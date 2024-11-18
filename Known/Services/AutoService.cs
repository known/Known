namespace Known.Services;

/// <summary>
/// 无代码模块服务接口。
/// </summary>
public interface IAutoService : IService
{
    /// <summary>
    /// 异步分页查询数据。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="info">删除对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info);

    /// <summary>
    /// 异步保存数据。
    /// </summary>
    /// <param name="info">保存表单对象。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info);

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">建表脚本对象。</param>
    /// <returns>创建结果。</returns>
    Task<Result> CreateTableAsync(AutoInfo<string> info);
}

class AutoService(HttpClient http) : ClientBase(http), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<Dictionary<string, object>>("/Auto/QueryModels", criteria);
    }

    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        return Http.PostAsync("/Auto/DeleteModels", info);
    }

    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        return Http.PostAsync("/Auto/SaveModel", info);
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Http.PostAsync("/Auto/CreateTable", info);
    }
}