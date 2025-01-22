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

    private Task<PagingResult<T>> QueryModelsAsync<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    {
        var datas = AppData.GetBizData<List<T>>(key) ?? [];
        if (filter != null)
            datas = filter.Invoke(datas);
        var result = datas == null
                   ? new PagingResult<T>(0, [])
                   : datas.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    private Task<Result> DeleteModelsAsync<T>(string key, List<T> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        var datas = AppData.GetBizData<List<T>>(key) ?? [];
        foreach (var info in infos)
        {
            var id = info.Property("Id");
            var item = datas.FirstOrDefault(d => d.Property("Id").Equals(id));
            if (item != null)
                datas.Remove(item);
        }
        AppData.SaveBizData(key, datas);
        return Result.SuccessAsync(Language.Success(Language.Delete));
    }

    private Task<Result> SaveModelAsync<T>(string key, T info)
    {
        var id = info.Property("Id");
        var datas = AppData.GetBizData<List<T>>(key) ?? [];
        var item = datas.FirstOrDefault(d => d.Property("Id").Equals(id));
        if (item != null)
            datas.Remove(item);
        datas.Add(info);
        AppData.SaveBizData(key, datas);
        return Result.SuccessAsync(Language.Success(Language.Save));
    }
}

partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
}