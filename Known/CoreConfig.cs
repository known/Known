namespace Known;

/// <summary>
/// 框架后端全局配置类。
/// </summary>
public partial class CoreConfig
{
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

    internal static void Load(InitialInfo info)
    {
        info.Settings[nameof(IsAuth)] = IsAuth;
        info.Settings[nameof(AuthStatus)] = AuthStatus;
    }

    internal static void Load(AdminInfo info)
    {
    }
}