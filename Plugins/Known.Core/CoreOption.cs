namespace Known;

/// <summary>
/// 身份认证方式枚举。
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// 无。
    /// </summary>
    None,
    /// <summary>
    /// 浏览器Session。
    /// </summary>
    Session,
    /// <summary>
    /// 浏览器Cookie。
    /// </summary>
    Cookie,
    /// <summary>
    /// 微软Identity。
    /// </summary>
    Identity
}

/// <summary>
/// 框架后端配置选项类。
/// </summary>
public class CoreOption
{
    internal static CoreOption Instance = new();
    internal static List<Func<List<CodeInfo>>> Funcs = [];
    internal static List<Func<Database, Task<List<CodeInfo>>>> FuncTasks = [];

    /// <summary>
    /// 取得App配置信息。
    /// </summary>
    public AppInfo App => Config.App;

    /// <summary>
    /// 取得或设置身份认证方式，默认浏览器Session。
    /// </summary>
    public AuthMode AuthMode { get; set; } = AuthMode.Session;

    /// <summary>
    /// 取得或设置响应数据是否启用压缩，默认禁用。
    /// </summary>
    public bool IsCompression { get; set; }

    /// <summary>
    /// 取得或设置是否根据Service动态生成WebApi，默认启用。
    /// </summary>
    public bool IsWebApi { get; set; } = true;

    /// <summary>
    /// 取得或设置MVC配置选项委托。
    /// </summary>
    public Action<MvcOptions> Mvc { get; set; }

    /// <summary>
    /// 取得或设置JSON配置选项委托。
    /// </summary>
    public Action<JsonOptions> Json { get; set; }

    /// <summary>
    /// 取得或设置文件内容类型字典。
    /// </summary>
    public Dictionary<string, string> ContentTypes { get; set; } = [];

    ///// <summary>
    ///// 添加附加数据字典代码表委托。
    ///// </summary>
    ///// <param name="func">代码表委托。</param>
    //public void AddCode(Func<List<CodeInfo>> func) => Funcs.Add(func);

    ///// <summary>
    ///// 添加附加数据字典代码表异步委托。
    ///// </summary>
    ///// <param name="func">代码表委托。</param>
    //public void AddCode(Func<Database, Task<List<CodeInfo>>> func) => FuncTasks.Add(func);
}