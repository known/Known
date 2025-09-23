namespace Known;

public partial class ServiceBase
{
    ///// <summary>
    ///// 异步查询模拟测试数据。
    ///// </summary>
    ///// <typeparam name="T">对象类型。</typeparam>
    ///// <param name="key">存储键。</param>
    ///// <param name="criteria">查询条件。</param>
    ///// <param name="filter">过滤器。</param>
    ///// <returns></returns>
    //protected static Task<PagingResult<T>> QueryModelsAsync<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    //{
    //    var result = AppData.QueryModels(key, criteria, filter);
    //    return Task.FromResult(result);
    //}

    ///// <summary>
    ///// 异步删除模拟测试数据。
    ///// </summary>
    ///// <typeparam name="T">对象类型。</typeparam>
    ///// <param name="key">存储键。</param>
    ///// <param name="infos">数据列表。</param>
    ///// <returns></returns>
    //protected Task<Result> DeleteModelsAsync<T>(string key, List<T> infos)
    //{
    //    if (infos == null || infos.Count == 0)
    //        return Result.ErrorAsync(Language.SelectOneAtLeast);

    //    var result = AppData.DeleteModels(key, infos);
    //    if (!result.IsValid)
    //        return Result.ErrorAsync(result.Message);

    //    return Result.SuccessAsync(Language.DeleteSuccess);
    //}

    ///// <summary>
    ///// 异步报错模拟测试数据。
    ///// </summary>
    ///// <typeparam name="T">对象类型。</typeparam>
    ///// <param name="key">存储键。</param>
    ///// <param name="info">数据信息。</param>
    ///// <returns></returns>
    //protected Task<Result> SaveModelAsync<T>(string key, T info)
    //{
    //    var result = AppData.SaveModel(key, info);
    //    if (!result.IsValid)
    //        return Result.ErrorAsync(result.Message);

    //    return Result.SuccessAsync(Language.SaveSuccess);
    //}
}