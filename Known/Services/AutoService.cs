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
    /// 异步获取数据。
    /// </summary>
    /// <param name="pageId">页面ID。</param>
    /// <param name="id">对象ID。</param>
    /// <returns></returns>
    Task<Dictionary<string, object>> GetModelAsync(string pageId, string id);

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
}

[Service]
class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        var key = criteria.GetParameter<string>(nameof(AutoInfo<string>.PageId));
        return QueryModelsAsync<Dictionary<string, object>>(key, criteria);
    }

    public Task<Dictionary<string, object>> GetModelAsync(string pageId, string id)
    {
        var model = AppData.GetModel<Dictionary<string, object>>(pageId, id);
        model ??= [];
        return Task.FromResult(model);
    }

    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        return DeleteModelsAsync(info.PageId, info.Data);
    }

    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        if (info.Model == null)
            return Result.ErrorAsync("数据不能为空！");

        var id = nameof(EntityBase.Id);
        if (!info.Model.ContainsKey(id))
            info.Model[id] = Utils.GetNextId();
        return SaveModelAsync(info.PageId, info.Model);
    }
}

[Client]
class AutoClient(HttpClient http) : ClientBase(http), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<Dictionary<string, object>>("/Auto/QueryModels", criteria);
    }

    public Task<Dictionary<string, object>> GetModelAsync(string pageId, string id)
    {
        return Http.GetAsync<Dictionary<string, object>>($"/Auto/GetModel?pageId={pageId}&id={id}");
    }

    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        return Http.PostAsync("/Auto/DeleteModels", info);
    }

    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        return Http.PostAsync("/Auto/SaveModel", info);
    }
}