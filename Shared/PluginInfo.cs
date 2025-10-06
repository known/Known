namespace Known;

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
    /// 取得或设置插件组件类型全名。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置插件组件参数配置JSON。
    /// </summary>
    public string Setting { get; set; }

    internal PluginInfo Clone()
    {
        return new PluginInfo { Id = Id, Type = Type, Setting = Setting };
    }
}

/// <summary>
/// 模块页面布局信息类。
/// </summary>
public class LayoutInfo
{
    /// <summary>
    /// 取得或设置布局类型，一栏、两栏、自定义。
    /// </summary>
    [Form(Type = nameof(FieldType.RadioList))]
    [Category(nameof(PageType))]
    [DisplayName("布局")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置页面分栏布局大小，如28，表示两列分别为20%和80%。
    /// </summary>
    [Form(Type = nameof(FieldType.RadioList))]
    [Category("19,28,37,46,55,91,82,73,64")]
    [DisplayName("分栏大小")]
    public string Spans { get; set; }

    /// <summary>
    /// 取得或设置自定义布局样式类。
    /// </summary>
    [DisplayName("样式类")]
    public string Custom { get; set; }

    internal LayoutInfo Clone()
    {
        return new LayoutInfo { Type = Type, Spans = Spans, Custom = Custom };
    }
}

/// <summary>
/// 页面布局类型枚举。
/// </summary>
public enum PageType
{
    /// <summary>
    /// 一栏布局。
    /// </summary>
    [Description("一栏")]
    None,
    /// <summary>
    /// 两栏布局。
    /// </summary>
    [Description("两栏")]
    Column,
    /// <summary>
    /// 自定义。
    /// </summary>
    [Description("自定义")]
    Custom
}