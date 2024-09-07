namespace Known.Cells;

/// <summary>
/// Sheet样式信息类。
/// </summary>
public class StyleInfo
{
    /// <summary>
    /// 取得或设置是否显示边框。
    /// </summary>
    public bool IsBorder { get; set; }

    /// <summary>
    /// 取得或设置是否粗体显示。
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// 取得或设置文本是否换行。
    /// </summary>
    public bool IsTextWrapped { get; set; }

    /// <summary>
    /// 取得或设置文本字体大小。
    /// </summary>
    public int? FontSize { get; set; }

    /// <summary>
    /// 取得或设置单元格自定义格式，如：yyyy-MM-dd。
    /// </summary>
    public string Custom { get; set; }

    /// <summary>
    /// 取得或设置单元格文本颜色。
    /// </summary>
    public Color? FontColor { get; set; }

    /// <summary>
    /// 取得或设置单元格背景颜色。
    /// </summary>
    public Color? BackgroundColor { get; set; }
}