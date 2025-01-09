namespace Known.Core;

/// <summary>
/// 框架后端配置类。
/// </summary>
public class CoreConfig
{
    /// <summary>
    /// 取得或设置系统安装时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task> OnInstallModules { get; set; }

    /// <summary>
    /// 取得或设置系统登录时，初始化系统模块数据方法委托。
    /// </summary>
    public static Func<Database, Task<List<ModuleInfo>>> OnInitialModules { get; set; }
}