using System.Reflection;

namespace Known;

/// <summary>
/// 后台权限管理配置选项类。
/// </summary>
public class AdminOption
{
    internal List<Action<List<SysModule>>> Modules { get; } = [];

    /// <summary>
    /// 取得或设置微信配置信息。
    /// </summary>
    public WeixinConfigInfo Weixin { get; set; }

    /// <summary>
    /// 向管理系统添加模块菜单列表。
    /// </summary>
    /// <param name="action">模块菜单列表。</param>
    public void AddModules(Action<List<SysModule>> action)
    {
        Modules.Add(action);
    }

    /// <summary>
    /// 添加后端工作流类程序集。
    /// </summary>
    /// <param name="assembly">应用程序集。</param>
    public void AddWorkFlows(Assembly assembly)
    {
        if (assembly == null)
            return;

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(BaseFlow)))
                BaseFlow.FlowTypes[item.Name] = item;
        }
    }
}