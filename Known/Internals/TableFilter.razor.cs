namespace Known.Internals;

/// <summary>
/// 表格列过滤组件类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public partial class TableFilter<TItem>
{
    /// <summary>
    /// 取得或设置表格模型配置信息。
    /// </summary>
    [Parameter] public TableModel<TItem> Table { get; set; }

    /// <summary>
    /// 取得或设置表格栏位信息。
    /// </summary>
    [Parameter] public ColumnInfo Item { get; set; }

    private async Task OnSearchAsync(List<QueryInfo> query)
    {
        Table.Criteria.Query = query;
        await Table.SearchAsync();
    }
}