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
public partial class ServiceBase(Context context) : IService
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
}

/// <summary>
/// 数据导入基类。
/// </summary>
/// <param name="context">导入上下文对象实例。</param>
public abstract class ImportBase(ImportContext context)
{
    /// <summary>
    /// 取得导入上下文对象实例。
    /// </summary>
    public ImportContext ImportContext { get; set; } = context;

    /// <summary>
    /// 取得系统上下文对象实例。
    /// </summary>
    public Context Context { get; } = context.Context;

    /// <summary>
    /// 取得上下文数据库对象实例。
    /// </summary>
    public Database Database { get; } = context.Database;

    /// <summary>
    /// 取得上下文语言对象实例。
    /// </summary>
    public Language Language => Context?.Language;

    /// <summary>
    /// 取得导入栏位信息列表。
    /// </summary>
    public List<ColumnInfo> Columns { get; } = [];

    /// <summary>
    /// 初始化导入栏位虚方法。
    /// </summary>
    public virtual void InitColumns() { }

    /// <summary>
    /// 异步执行导入数据虚方法。
    /// </summary>
    /// <param name="file">导入文件对象。</param>
    /// <returns>导入结果。</returns>
    public virtual Task<Result> ExecuteAsync(AttachInfo file) => Result.SuccessAsync("");
}

/// <summary>
/// 数据导入上下文类。
/// </summary>
public class ImportContext
{
    /// <summary>
    /// 取得或设置系统上下文对象实例。
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    /// 取得或设置数据库访问实例。
    /// </summary>
    public Database Database { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得是否是字典类型。
    /// </summary>
    public bool IsDictionary => !string.IsNullOrWhiteSpace(BizId) && BizId.StartsWith(Config.AutoBizIdPrefix);

    /// <summary>
    /// 取得业务参数。
    /// </summary>
    public string BizParam => GetBizIdValue(1);

    /// <summary>
    /// 取得页面ID。
    /// </summary>
    public string PageId => GetBizIdValue(1);

    /// <summary>
    /// 取得插件ID。
    /// </summary>
    public string PluginId => GetBizIdValue(2);

    private string GetBizIdValue(int index)
    {
        if (string.IsNullOrWhiteSpace(BizId))
            return string.Empty;

        var bizIds = BizId.Split('_');
        if (bizIds.Length > index)
            return bizIds[index];

        return string.Empty;
    }
}