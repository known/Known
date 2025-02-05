namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public partial interface IAdminService : IService
{
}

partial class AdminService(Context context) : ServiceBase(context), IAdminService
{
    private static readonly Dictionary<string, string> Configs = [];

    private static Task<PagingResult<T>> QueryModelsAsync<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    {
        var result = AppData.QueryModels(key, criteria, filter);
        return Task.FromResult(result);
    }

    private Task<Result> DeleteModelsAsync<T>(string key, List<T> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        var result = AppData.DeleteModels(key, infos);
        if (!result.IsValid)
            return Result.ErrorAsync(result.Message);

        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    private Task<Result> SaveModelAsync<T>(string key, T info)
    {
        var result = AppData.SaveModel(key, info);
        if (!result.IsValid)
            return Result.ErrorAsync(result.Message);

        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
}