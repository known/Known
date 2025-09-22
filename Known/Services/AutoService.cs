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

[WebApi, Service]
class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public async Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        var database = Database;
        var pageId = criteria.GetParameter<string>(nameof(AutoInfo<object>.PageId));
        var pluginId = criteria.GetParameter<string>(nameof(AutoInfo<object>.PluginId));
        var autoPage = await database.GetAutoPageAsync(pageId, pluginId);
        var idField = autoPage?.IdField;
        var tableName = criteria.GetParameter<string>("TableName");
        if (string.IsNullOrWhiteSpace(tableName))
            tableName = autoPage?.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return new PagingResult<Dictionary<string, object>>();

        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [idField];

        if (autoPage?.PageType == AutoPageType.NewTable)
            criteria.SetQuery(nameof(EntityBase.CompNo), QueryType.Equal, CurrentUser?.CompNo);
        var db = await database.GetDatabaseAsync(autoPage);
        return await db.QueryPageAsync(tableName, criteria);
    }

    public async Task<Dictionary<string, object>> GetModelAsync(string pageId, string id)
    {
        var database = Database;
        var autoPage = await database.GetAutoPageAsync(pageId, "");
        var tableName = autoPage?.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return [];

        var db = await database.GetDatabaseAsync(autoPage);
        return await db.QueryByIdAsync(tableName, id);
    }

    public async Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        var database = Database;
        var autoPage = await database.GetAutoPageAsync(info.PageId, info.PluginId);
        var idField = autoPage?.IdField;
        var tableName = autoPage?.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        if (info.Data == null || info.Data.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var database1 = await database.GetDatabaseAsync(autoPage);
        var result = await database1.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in info.Data)
            {
                var id = item.GetValue<string>(idField);
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                await db.DeleteFlowAsync(id);
                await db.DeleteFilesAsync(id, oldFiles);
                await db.DeleteAsync(tableName, id, idField);
            }
        });
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        var database = Database;
        var autoPage = await database.GetAutoPageAsync(info.PageId, info.PluginId);
        var tableName = autoPage?.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        var idField = autoPage?.IdField;
        var entity = autoPage?.ToEntity();
        var model = info.Model;
        var vr = DataHelper.Validate(Context, entity, model);
        if (!vr.IsValid)
            return vr;

        model.ReplaceDataPlaceholder(CurrentUser);
        var database1 = await database.GetDatabaseAsync(autoPage);
        return await database1.TransactionAsync(Language.Save, async db =>
        {
            var id = model.GetValue<string>(idField);
            if (string.IsNullOrWhiteSpace(id))
                id = Utils.GetNextId();
            if (info.Files != null && info.Files.Count > 0)
            {
                foreach (var file in info.Files)
                {
                    var files = info.Files.GetAttachFiles(file.Key, tableName);
                    await db.AddFilesAsync(files, id, key => model[file.Key] = key);
                }
            }
            model.SetValue(idField, id);
            await db.SaveAsync(tableName, model, idField, autoPage?.PageType == AutoPageType.NewTable);
        }, model);
    }
}