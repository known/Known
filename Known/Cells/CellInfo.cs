namespace Known.Cells;

/// <summary>
/// 单元格信息类。
/// </summary>
public class CellInfo
{
    /// <summary>
    /// 取得或设置单元格名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置单元格行号。
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// 取得或设置单元格列号。
    /// </summary>
    public int Column { get; set; }
}