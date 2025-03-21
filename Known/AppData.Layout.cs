namespace Known;

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
}