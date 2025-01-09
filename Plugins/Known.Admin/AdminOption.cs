namespace Known;

/// <summary>
/// 后台权限管理配置选项类。
/// </summary>
public class AdminOption
{
    internal static AdminOption Instance = new();
    internal List<Action<List<SysModule>>> Modules { get; } = [];

    /// <summary>
    /// 取得或设置代码配置信息。
    /// </summary>
    public CodeConfigInfo Code { get; set; }

    /// <summary>
    /// 向管理系统添加模块菜单列表。
    /// </summary>
    /// <param name="action">模块菜单列表。</param>
    public void AddModules(Action<List<SysModule>> action)
    {
        Modules.Add(action);
    }
}