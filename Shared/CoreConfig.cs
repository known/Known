namespace Known;

/// <summary>
/// 框架后端全局配置类。
/// </summary>
public class CoreConfig
{
    private CoreConfig() { }

    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
    // 取得后端任务类字典。
    internal static ConcurrentDictionary<string, Type> TaskTypes { get; } = [];
    // 取得后端导入类字典。
    internal static ConcurrentDictionary<string, Type> ImportTypes { get; } = [];
    // 取得后端工作流类字典。
    internal static ConcurrentDictionary<string, Type> FlowTypes { get; } = [];

    internal static void Load(InitialInfo info)
    {
        info.Settings[nameof(IsAuth)] = IsAuth;
        info.Settings[nameof(AuthStatus)] = AuthStatus;
    }

    internal static void Load(AdminInfo info) { }

    /// <summary>
    /// 取得或设置默认用户系统设置信息对象。
    /// </summary>
    public static UserSettingInfo UserSetting { get; set; } = new();

    /// <summary>
    /// 取得或设置获取初始化信息后附加操作委托。
    /// </summary>
    public static Func<Database, InitialInfo, Task> OnInitial { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Func<Database, AdminInfo, Task> OnAdmin { get; set; }

    /// <summary>
    /// 取得系统启动时间。
    /// </summary>
    public static DateTime StartTime { get; internal set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    public static SystemInfo System { get; set; }

    /// <summary>
    /// 取得框架自动解析服务接口生成的WebApi方法信息列表。
    /// </summary>
    public static ConcurrentBag<ApiMethodInfo> ApiMethods { get; } = [];

    /// <summary>
    /// 取得或设置超级管理员用户密码。
    /// </summary>
    public static string SuperPassword { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的产品ID。
    /// </summary>
    public static string ProductId { get; set; }

    /// <summary>
    /// 取得或设置系统授权验证方法，如果设置，则页面会先校验系统License，不通过，则显示框架内置的未授权面板。
    /// </summary>
    public static Func<SystemInfo, Result> CheckSystem { get; set; }

    /// <summary>
    /// 取得或设置获取用户角色模块ID列表委托。
    /// </summary>
    public static Func<Database, string, Task<List<string>>> OnRoleModule { get; set; }

    /// <summary>
    /// 取得或设置系统安装后附加操作委托。
    /// </summary>
    public static Func<Database, InstallInfo, SystemInfo, Task> OnInstall { get; set; }

    /// <summary>
    /// 取得或设置系统安装时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task> OnInstallModules { get; set; }

    /// <summary>
    /// 取得或设置保存新用户时委托。
    /// </summary>
    public static Action<Database, SysUser> OnNewUser { get; set; }

    /// <summary>
    /// 取得或设置获取代码表信息列表委托。
    /// </summary>
    public static Func<Database, Task<List<CodeInfo>>> OnCodeTable { get; set; }

    /// <summary>
    /// 取得或设置无代码插件数据服务关联数据库委托，用于根据插件获取关联的数据库对象。
    /// </summary>
    public static Func<Database, AutoPageInfo, Task<Database>> OnDatabase { get; set; }

    /// <summary>
    /// 取得或设置迁移系统配置数据委托。
    /// </summary>
    public static Func<Database, Task> OnMigrateAppData { get; set; }

    /// <summary>
    /// 取得或设置用户注册前验证逻辑委托。
    /// </summary>
    public static Func<Database, RegisterFormInfo, Task<Result>> OnRegistering { get; set; }

    /// <summary>
    /// 取得或设置用户注册后逻辑委托。
    /// </summary>
    public static Func<Database, SysUser, Task> OnRegistered { get; set; }

    /// <summary>
    /// 取得或设置用户登录前验证逻辑委托。
    /// </summary>
    public static Func<Database, LoginFormInfo, Task<Result>> OnLoging { get; set; }

    /// <summary>
    /// 取得或设置用户登录后逻辑委托。
    /// </summary>
    public static Func<Database, UserInfo, Task> OnLoged { get; set; }

    /// <summary>
    /// 取得或设置激活系统委托。
    /// </summary>
    public static Func<Database, ActiveInfo, Task<Result>> OnActiveSystem { get; set; }

    /// <summary>
    /// 取得激活授权组件的验证委托列表。
    /// </summary>
    public static List<Func<ActiveInfo, Result>> Actives { get; } = [];

    /// <summary>
    /// 检查系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>检查结果。</returns>
    public static Result CheckSystemInfo(SystemInfo info)
    {
        if (CheckSystem == null)
            return Result.Success("");

        info.ProductId = ProductId;
        var result = CheckSystem.Invoke(info);
        IsAuth = result.IsValid;
        AuthStatus = result.Message;
        return result;
    }
}