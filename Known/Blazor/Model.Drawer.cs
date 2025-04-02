namespace Known.Blazor;

/// <summary>
/// 抽屉配置模型信息类。
/// </summary>
public class DrawerModel
{
    /// <summary>
    /// 取得或设置抽屉CSS类名。
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 取得或设置抽屉标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置抽屉宽度。
    /// </summary>
    public string Width { get; set; }

    /// <summary>
    /// 取得或设置抽屉内容呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }
}