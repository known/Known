namespace Known.Reports;

/// <summary>
/// 报表类型表单。
/// </summary>
public class ReportTypeForm : AntForm<SysReport> { }

/// <summary>
/// 列配置表格。
/// </summary>
public class ColumnConfigTable : AntTable<ColumnConfig> { }

/// <summary>
/// 报表设置表单。
/// </summary>
public partial class ReportForm
{
    private ReportConfig reportConfig;
    private ChartConfig chartConfig;
    private TableConfig tableConfig;

    private readonly List<CodeInfo> Types =
    [
        new CodeInfo("Table", "表格"),
        new CodeInfo("Chart", "图表"),
        new CodeInfo("Combination", "组合")
    ];
    private readonly List<CodeInfo> ChartTypes =
    [
        new CodeInfo("bar", "柱状图"),
        new CodeInfo("line", "折线图"),
        new CodeInfo("pie", "饼图"),
        new CodeInfo("area", "面积图"),
        new CodeInfo("scatter", "散点图")
    ];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        LoadConfig();
    }

    private void LoadConfig()
    {
        reportConfig = !string.IsNullOrWhiteSpace(Model.Data.Config)
            ? Utils.FromJson<ReportConfig>(Model.Data.Config) ?? new ReportConfig()
            : new ReportConfig();
        chartConfig = reportConfig.Charts?.FirstOrDefault() ?? new ChartConfig();
        tableConfig = reportConfig.Tables?.FirstOrDefault() ?? new TableConfig();
    }

    private void OnAddColumn()
    {
        tableConfig.Columns.Add(new ColumnConfig());
        StateChanged();
    }

    private void OnDeleteColumn(ColumnConfig column)
    {
        tableConfig.Columns.Remove(column);
        StateChanged();
    }
}