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
    /// 异步获取代码配置信息。
    /// </summary>
    /// <returns>代码配置信息。</returns>
    Task<CodeConfigInfo> GetCodeConfigAsync();

    /// <summary>
    /// 异步保存代码。
    /// </summary>
    /// <param name="info">代码信息。</param>
    /// <returns>创建结果。</returns>
    Task<Result> SaveCodeAsync(AutoInfo<string> info);

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">建表脚本对象。</param>
    /// <returns>创建结果。</returns>
    Task<Result> CreateTableAsync(AutoInfo<string> info);
}

[Service]
class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        var key = criteria.GetParameter<string>(nameof(AutoInfo<string>.PageId));
        return QueryModelsAsync<Dictionary<string, object>>(key, criteria);
    }

    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        return DeleteModelsAsync(info.PageId, info.Data);
    }

    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        var id = nameof(EntityBase.Id);
        if (!info.Model.ContainsKey(id))
            info.Model[id] = Utils.GetNextId();
        return SaveModelAsync(info.PageId, info.Model);
    }

    public Task<CodeConfigInfo> GetCodeConfigAsync()
    {
        return Task.FromResult(new CodeConfigInfo());
    }

    public Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        return Result.SuccessAsync("保存成功！");
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Result.SuccessAsync("执行成功！");
    }
}

[Client]
class AutoClient(HttpClient http) : ClientBase(http), IAutoService
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

    public Task<CodeConfigInfo> GetCodeConfigAsync()
    {
        return Http.GetAsync<CodeConfigInfo>("/Auto/GetCodeConfig");
    }

    public Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        return Http.PostAsync("/Auto/SaveCode", info);
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Http.PostAsync("/Auto/CreateTable", info);
    }
}