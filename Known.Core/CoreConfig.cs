namespace Known;

/// <summary>
/// 框架后端全局配置类。
/// </summary>
public class CoreConfig
{
    private CoreConfig() { }

    /// <summary>
    /// 取得或设置用户登录前验证逻辑委托。
    /// </summary>
    public static Func<Database, LoginFormInfo, Task<Result>> OnLoging { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Func<Database, AdminInfo, Task> OnAdmin { get; set; }
}