namespace Known.Core;

/// <summary>
/// 身份认证方式枚举。
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// 浏览器Session。
    /// </summary>
    Session,
    /// <summary>
    /// 浏览器Cookie。
    /// </summary>
    Cookie,
    /// <summary>
    /// 微软Identity。
    /// </summary>
    Identity
}

/// <summary>
/// 框架后端配置选项类。
/// </summary>
public class CoreOption
{
    internal static List<Assembly> Assemblies { get; } = [];

    /// <summary>
    /// 取得或设置身份认证方式，默认浏览器Session。
    /// </summary>
    public AuthMode AuthMode { get; set; } = AuthMode.Session;

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
    /// <param name="assembly">应用程序集。</param>
    public void AddAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Assemblies.Exists(a => a.FullName == assembly.FullName))
            return;

        Assemblies.Add(assembly);
    }
}