namespace Known.Core;

/// <summary>
/// 框架后端配置选项类。
/// </summary>
public class CoreOption
{
    internal static List<Assembly> Assemblies { get; } = [];

    /// <summary>
    /// 取得或设置响应数据是否启用压缩，默认禁用。
    /// </summary>
    public bool IsCompression { get; set; }

    /// <summary>
    /// 取得或设置是否动态生成WebApi，默认启用。
    /// </summary>
    public bool IsAddWebApi { get; set; } = true;

    /// <summary>
    /// 添加后端程序集，用于获取导入类、工作流类、数据库表脚本。
    /// </summary>
    /// <param name="assembly"></param>
    public void AddAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        Assemblies.Add(assembly);
        foreach (var item in assembly.GetTypes())
        {
            if (item.IsAssignableTo(typeof(ImportBase)))
                ImportHelper.ImportTypes[item.Name] = item;
            else if (item.IsAssignableTo(typeof(BaseFlow)))
                BaseFlow.FlowTypes[item.Name] = item;
        }
    }
}