namespace Known.Blazor;

/// <summary>
/// 列表项模型信息类。
/// </summary>
/// <param name="id">列表项ID。</param>
/// <param name="title">列表项标题。</param>
public class ItemModel(string id, string title)
{
    /// <summary>
    /// 取得列表项ID。
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 取得列表项标题。
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// 取得或设置列表项子标题。
    /// </summary>
    public string SubTitle { get; set; }

    /// <summary>
    /// 取得或设置列表项描述信息。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置项目是否显示。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }

    /// <summary>
    /// 取得或设置列表项表单配置模型。
    /// </summary>
    public TableModel Table { get; set; }
}