namespace Known.Plugins;

/// <summary>
/// 插件菜单信息类。
/// </summary>
public class PluginMenuInfo
{
    internal PluginMenuInfo(Type type, PluginAttribute attribute)
    {
        Id = type.FullName;
        Type = type;
        Attribute = attribute;
    }

    /// <summary>
    /// 取得或设置插件ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置插件组件类型。
    /// </summary>
    public Type Type { get; set; }

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
    public bool IsNavComponent => Type.IsSubclassOf(typeof(BaseNav));

    internal bool IsDev => Attribute is DevPluginAttribute;
    internal bool IsNav => Attribute is NavPluginAttribute;
    internal bool IsPage => Attribute is PagePluginAttribute;
}