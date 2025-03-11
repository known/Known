namespace Known;

/// <summary>
/// 前后端交互服务类接口。
/// </summary>
public interface IService
{
    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    Context Context { get; set; }
}

/// <summary>
/// 抽象客户端基类。
/// </summary>
/// <param name="http">HTTP客户端。</param>
public abstract class ClientBase(HttpClient http) : IService
{
    /// <summary>
    /// 取得HTTP客户端对象。
    /// </summary>
    public HttpClient Http
    {
        get
        {
            var user = Context?.CurrentUser;
            var token = user != null ? user.Token : "none";
            http.DefaultRequestHeaders.Remove(Constants.KeyToken);
            http.DefaultRequestHeaders.Add(Constants.KeyToken, token);
            return http;
        }
    }

    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    public Context Context { get; set; }
}

/// <summary>
/// 抽象业务服务基类。
/// </summary>
/// <param name="context">系统上下文对象。</param>
public abstract class ServiceBase(Context context) : IService
{
    /// <summary>
    /// 取得当前系统配置信息。
    /// </summary>
    public AppInfo App => Config.App;

    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    public Context Context { get; set; } = context;

    /// <summary>
    /// 取得当前用户信息。
    /// </summary>
    public UserInfo CurrentUser => Context.CurrentUser;

    /// <summary>
    /// 取得当前语言对象。
    /// </summary>
    public Language Language => Context.Language;

    /// <summary>
    /// 取得数据库访问实例。
    /// </summary>
    public virtual Database Database
    {
        get
        {
            if (App.IsClient)
                return null;

            var db = Database.Create();
            db.User = CurrentUser;
            db.Context = Context;
            return db;
        }
    }

    /// <summary>
    /// 异步查询模拟测试数据。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="key">存储键。</param>
    /// <param name="criteria">查询条件。</param>
    /// <param name="filter">过滤器。</param>
    /// <returns></returns>
    protected static Task<PagingResult<T>> QueryModelsAsync<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    {
        var result = AppData.QueryModels(key, criteria, filter);
        return Task.FromResult(result);
    }

    /// <summary>
    /// 异步删除模拟测试数据。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="key">存储键。</param>
    /// <param name="infos">数据列表。</param>
    /// <returns></returns>
    protected Task<Result> DeleteModelsAsync<T>(string key, List<T> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        var result = AppData.DeleteModels(key, infos);
        if (!result.IsValid)
            return Result.ErrorAsync(result.Message);

        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    /// <summary>
    /// 异步报错模拟测试数据。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="key">存储键。</param>
    /// <param name="info">数据信息。</param>
    /// <returns></returns>
    protected Task<Result> SaveModelAsync<T>(string key, T info)
    {
        var result = AppData.SaveModel(key, info);
        if (!result.IsValid)
            return Result.ErrorAsync(result.Message);

        return Result.SuccessAsync(Language.SaveSuccess);
    }
}