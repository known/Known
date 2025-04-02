namespace Known;

/// <summary>
/// 框架后端全局配置类。
/// </summary>
public class CoreConfig
{
    private CoreConfig() { }

    /// <summary>
    /// 取得或设置自定义用户业务逻辑处理者实例。
    /// </summary>
    public static UserHandler UserHandler { get; set; }

    /// <summary>
    /// 取得或设置用户登录前验证逻辑委托。
    /// </summary>
    public static Func<Database, LoginFormInfo, Task<Result>> OnLoging { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Func<Database, AdminInfo, Task> OnAdmin { get; set; }

    /// <summary>
    /// 取得或设置迁移系统配置数据委托。
    /// </summary>
    public static Func<Database, Task> OnMigrateAppData { get; set; }

    /// <summary>
    /// 取得或设置无代码插件数据服务关联数据库委托，用于根据插件获取关联的数据库对象。
    /// </summary>
    public static Func<Database, AutoPageInfo, Database> OnDatabase { get; set; }
}