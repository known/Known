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
    public bool IsAddWebApi { get; set; } = true;

    /// <summary>
    /// 取得或设置Web日志保留天数，默认7天。
    /// </summary>
    public int WebLogDays { get; set; } = 7;

    /// <summary>
    /// 取得或设置MVC配置选项委托。
    /// </summary>
    public Action<MvcOptions> Mvc { get; set; }

    /// <summary>
    /// 取得或设置JSON配置选项委托。
    /// </summary>
    public Action<JsonOptions> Json { get; set; }

    /// <summary>
    /// 取得或设置数据库访问配置选项委托。
    /// </summary>
    public Action<DatabaseOption> Database { get; set; }

    /// <summary>
    /// 取得或设置微信配置信息。
    /// </summary>
    public WeixinConfigInfo Weixin { get; set; }

    /// <summary>
    /// 取得或设置代码配置信息。
    /// </summary>
    public CodeConfigInfo Code { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的产品ID。
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置系统授权验证方法，如果设置，则页面会先校验系统License，不通过，则显示框架内置的未授权面板。
    /// </summary>
    public Func<SystemInfo, Result> CheckSystem { get; set; }

    /// <summary>
    /// 检查系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>检查结果。</returns>
    public Result CheckSystemInfo(SystemInfo info)
    {
        if (CheckSystem == null)
            return Result.Success("");

        var result = CheckSystem.Invoke(info);
        Config.IsAuth = result.IsValid;
        Config.AuthStatus = result.Message;
        return result;
    }

    /// <summary>
    /// 添加后端程序集，自动识别导入和工作流类。
    /// </summary>
    /// <param name="assembly">应用程序集。</param>
    public void AddAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(ImportBase)))
                ImportHelper.ImportTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(FlowBase)))
                FlowBase.FlowTypes[item.Name] = item;
        }
    }

    /// <summary>
    /// 添加附加数据字典代码表委托。
    /// </summary>
    /// <param name="func">代码表委托。</param>
    public void AddCode(Func<List<CodeInfo>> func) => Funcs.Add(func);

    /// <summary>
    /// 添加附加数据字典代码表异步委托。
    /// </summary>
    /// <param name="func">代码表委托。</param>
    public void AddCode(Func<Database, Task<List<CodeInfo>>> func) => FuncTasks.Add(func);
}