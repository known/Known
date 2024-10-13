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
/// 抽象业务服务基类。
/// </summary>
/// <param name="context">系统上下文对象。</param>
public abstract class ServiceBase(Context context) : IService
{
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
    /// 取得依赖注入服务提供者。
    /// </summary>
    public IServiceProvider ServiceProvider => Config.ServiceProvider;

    /// <summary>
    /// 取得数据库访问实例。
    /// </summary>
    protected virtual Database Database
    {
        get
        {
            var db = Database.Create();
            db.User = CurrentUser;
            db.Context = Context;
            return db;
        }
    }
}