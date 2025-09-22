namespace Known;

/// <summary>
/// 框架后端全局配置类。
/// </summary>
public class CoreConfig
{
    private CoreConfig() { }

    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }

    internal static void Load(InitialInfo info)
    {
        info.Settings[nameof(IsAuth)] = IsAuth;
        info.Settings[nameof(AuthStatus)] = AuthStatus;
    }

    internal static void Load(AdminInfo info)
    {
    }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    public static SystemInfo System { get; set; }

    /// <summary>
    /// 取得或设置用户注册前验证逻辑委托。
    /// </summary>
    public static Func<Database, RegisterFormInfo, Task<Result>> OnRegistering { get; set; }

    /// <summary>
    /// 取得或设置用户注册后逻辑委托。
    /// </summary>
    public static Func<Database, UserInfo, Task> OnRegistered { get; set; }

    /// <summary>
    /// 取得或设置用户登录前验证逻辑委托。
    /// </summary>
    public static Func<Database, LoginFormInfo, Task<Result>> OnLoging { get; set; }

    /// <summary>
    /// 取得或设置用户登录后逻辑委托。
    /// </summary>
    public static Func<Database, UserInfo, Task> OnLoged { get; set; }

    /// <summary>
    /// 取得或设置获取初始化信息后附加操作委托。
    /// </summary>
    public static Func<Database, InitialInfo, Task> OnInitial { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Func<Database, AdminInfo, Task> OnAdmin { get; set; }

    /// <summary>
    /// 取得或设置迁移系统配置数据委托。
    /// </summary>
    public static Func<Database, Task> OnMigrateAppData { get; set; }

    /// <summary>
    /// 取得或设置激活系统委托。
    /// </summary>
    public static Func<Database, ActiveInfo, Task<Result>> OnActiveSystem { get; set; }

    /// <summary>
    /// 取得激活授权组件的验证委托列表。
    /// </summary>
    public static List<Func<ActiveInfo, Result>> Actives { get; } = [];
}