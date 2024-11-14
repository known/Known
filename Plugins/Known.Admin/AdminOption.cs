namespace Known.Admin;

/// <summary>
/// 后台权限管理配置选项类。
/// </summary>
public class AdminOption
{
    internal List<Action<List<SysModule>>> AddActions { get; } = [];

    /// <summary>
    /// 向管理系统添加模块菜单列表。
    /// </summary>
    /// <param name="action">模块菜单列表。</param>
    public void AddModules(Action<List<SysModule>> action)
    {
        AddActions.Add(action);
    }
}