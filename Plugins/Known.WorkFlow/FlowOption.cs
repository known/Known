using System.Reflection;

namespace Known.WorkFlow;

/// <summary>
/// 工作流配置选项类。
/// </summary>
public class FlowOption
{
    internal static List<Assembly> Assemblies { get; } = [];

    /// <summary>
    /// 添加后端程序集，用于获取工作流类、数据库表脚本。
    /// </summary>
    /// <param name="assembly">应用程序集。</param>
    public void AddAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        Assemblies.Add(assembly);
        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(BaseFlow)))
                BaseFlow.FlowTypes[item.Name] = item;
        }
    }
}