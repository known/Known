namespace Known.Services;

[WebApi]
class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<Dictionary<string, object>>());
    }

    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        return Result.SuccessAsync("添加成功！");
    }

    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        return Result.SuccessAsync("保存成功！");
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Result.SuccessAsync("执行成功！");
    }
}