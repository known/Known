namespace Known.Blazor;

/// <summary>
/// 表格汇总单元格信息类。
/// </summary>
/// <typeparam name="TItem">数据类型。</typeparam>
public class TableSumCellInfo<TItem> where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表格模型。
    /// </summary>
    public TableModel<TItem> Table { get; set; }

    /// <summary>
    /// 取得或设置汇总列信息。
    /// </summary>
    public ColumnInfo Column { get; set; }
}