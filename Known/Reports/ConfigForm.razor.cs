namespace Known.Reports;

/// <summary>
/// 报表块配置弹窗组件类。
/// </summary>
public partial class ConfigForm
{
    /// <summary>
    /// 取得或设置要配置的报表块。
    /// </summary>
    [Parameter] public ReportBlock Block { get; set; }

    private void OnAddColumn()
    {
        Block?.Table?.Columns?.Add(new ColumnConfig());
        StateChanged();
    }

    private void OnDeleteColumn(ColumnConfig column)
    {
        Block?.Table?.Columns?.Remove(column);
        StateChanged();
    }
}
