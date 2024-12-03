using System.Reflection;

namespace Known;

/// <summary>
/// 后台权限管理配置选项类。
/// </summary>
public class AdminOption
{
    internal static AdminOption Instance = new();
    internal List<Action<List<SysModule>>> Modules { get; } = [];

    /// <summary>
    /// 取得或设置微信配置信息。
    /// </summary>
    public WeixinConfigInfo Weixin { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的产品ID。
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置【关于系统】模块显示的版权信息。
    /// </summary>
    public string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";

    /// <summary>
    /// 取得或设置【关于系统】模块显示的软件许可信息。
    /// </summary>
    public string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";

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
        AdminConfig.IsAuth = result.IsValid;
        AdminConfig.AuthStatus = result.Message;
        return result;
    }

    /// <summary>
    /// 向管理系统添加模块菜单列表。
    /// </summary>
    /// <param name="action">模块菜单列表。</param>
    public void AddModules(Action<List<SysModule>> action)
    {
        Modules.Add(action);
    }

    /// <summary>
    /// 添加后端程序集，自动识别工作流类。
    /// </summary>
    /// <param name="assembly">应用程序集。</param>
    public void AddAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(FlowBase)))
                FlowBase.FlowTypes[item.Name] = item;
        }
    }
}