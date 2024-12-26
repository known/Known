namespace Known;

/// <summary>
/// 框架配置数据信息类。
/// </summary>
public partial class AppDataInfo
{
    /// <summary>
    /// 取得或设置顶部导航信息列表。
    /// </summary>
    public List<PluginInfo> TopNavs { get; set; } = [];

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];
}

/// <summary>
/// 框架插件信息类。
/// </summary>
public class PluginInfo
{
    /// <summary>
    /// 取得或设置插件实例ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置插件类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置插件配置JSON。
    /// </summary>
    public string Setting { get; set; }

    /// <summary>
    /// 获取指定类型的插件对象。
    /// </summary>
    /// <typeparam name="T">插件类型。</typeparam>
    /// <returns>插件对象。</returns>
    public T AsPlugin<T>() => Utils.FromJson<T>(Setting);
}

/// <summary>
/// 框架模块信息类。
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置类型（Menu/Page/Link）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置目标（None/Blank/IFrame）。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public List<PluginInfo> Plugins { get; set; } = [];

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 添加插件配置信息。
    /// </summary>
    /// <typeparam name="T">插件配置类型。</typeparam>
    /// <param name="plugin">插件配置信息。</param>
    /// <param name="id">插件实例ID。</param>
    public void AddPlugin<T>(T plugin, string id = "")
    {
        Plugins ??= [];
        Plugins.AddPlugin(plugin, id);
    }

    /// <summary>
    /// 根据ID获取插件配置信息。
    /// </summary>
    /// <typeparam name="T">插件配置类型。</typeparam>
    /// <param name="id">插件实例ID。</param>
    /// <returns>插件配置信息。</returns>
    public T GetPlugin<T>(string id = "")
    {
        if (Plugins == null)
            return default;

        return Plugins.GetPlugin<T>(id);
    }

    /// <summary>
    /// 获取模块的字符串表示。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name}({Url})";
    }
}