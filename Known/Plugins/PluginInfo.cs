namespace Known.Plugins;

/// <summary>
/// 插件信息类。
/// </summary>
public class PluginInfo
{
    internal PluginInfo(Type type, PluginAttribute attribute)
    {
        Id = type.FullName;
        Component = type;
        Attribute = attribute;
    }

    /// <summary>
    /// 取得或设置插件ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置插件组件类型。
    /// </summary>
    public Type Component { get; set; }

    /// <summary>
    /// 取得或设置插件特性。
    /// </summary>
    public PluginAttribute Attribute { get; set; }

    /// <summary>
    /// 取得或设置插件组件URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得是否是导航组件。
    /// </summary>
    public bool IsNavComponent => Component.IsSubclassOf(typeof(BaseNav));

    internal bool IsDev => Attribute is DevPluginAttribute;
    internal bool IsNav => Attribute is NavPluginAttribute;
    internal bool IsMenu => Attribute is MenuPluginAttribute;
    internal bool IsPage => Attribute is PagePluginAttribute;
}