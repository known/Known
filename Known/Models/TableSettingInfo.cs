namespace Known.Models;

/// <summary>
/// 表格列设置信息类。
/// </summary>
public class TableSettingInfo
{
    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位固定位置(left/right)。
    /// </summary>
    public string Fixed { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见。
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置栏位显示顺序。
    /// </summary>
    public int Sort { get; set; }
}