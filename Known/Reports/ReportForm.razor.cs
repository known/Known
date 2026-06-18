namespace Known.Reports;

/// <summary>
/// 报表类型表单组件类。
/// </summary>
public class ReportTypeForm : AntForm<SysReport> { }

/// <summary>
/// 列配置表格组件类。
/// </summary>
public class ColumnConfigTable : AntTable<ColumnConfig> { }

/// <summary>
/// 报表设置表单类。
/// </summary>
public partial class ReportForm
{
    private ReportConfig reportConfig;
    private List<ReportBlock> blocks = [];
    private ReportBlock selectedBlock;

    private readonly List<CodeInfo> Types =
    [
        new("Custom", "自动"),
        new("Table", "表格"),
        new("Chart", "图表"),
        new("Combination", "组合")
    ];

    private readonly List<CodeInfo> ChartTypes =
    [
        new("bar", "柱状图"),
        new("line", "折线图"),
        new("pie", "饼图"),
        new("area", "面积图"),
        new("scatter", "散点图")
    ];

    private readonly List<CodeInfo> SourceTypes =
    [
        new("Sample", "示例数据"),
        new("Table", "数据库表"),
        new("SQL", "SQL语句"),
        new("Api", "API接口")
    ];

    private readonly List<CodeInfo> RequestMethods =
    [
        new("GET", "GET"),
        new("POST", "POST")
    ];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        LoadConfig();
        Model.OnSaving = data =>
        {
            SaveConfig();
            return Task.FromResult(true);
        };
    }

    private void SaveConfig()
    {
        reportConfig.Blocks = blocks;
        Model.Data.Config = Utils.ToJson(reportConfig);
    }

    private void LoadConfig()
    {
        reportConfig = !string.IsNullOrWhiteSpace(Model.Data.Config)
            ? Utils.FromJson<ReportConfig>(Model.Data.Config) ?? new ReportConfig()
            : new ReportConfig();

        blocks = reportConfig.Blocks ?? [];

        if (blocks.Count == 0)
        {
            MigrateLegacyConfig();
        }

        selectedBlock = blocks.FirstOrDefault();
        SyncReportType();
    }

    private void MigrateLegacyConfig()
    {
        if (reportConfig.Charts?.Count > 0)
        {
            foreach (var chart in reportConfig.Charts)
            {
                blocks.Add(new ReportBlock
                {
                    BlockType = "Chart",
                    Title = chart.Title,
                    Width = 12,
                    Chart = chart,
                    DataSource = chart.DataSource ?? new DataSourceConfig()
                });
            }
        }

        if (reportConfig.Tables?.Count > 0)
        {
            foreach (var table in reportConfig.Tables)
            {
                blocks.Add(new ReportBlock
                {
                    BlockType = "Table",
                    Title = "数据表格",
                    Width = 12,
                    Table = table,
                    DataSource = table.DataSource ?? new DataSourceConfig()
                });
            }
        }

        reportConfig.Blocks = blocks;
    }

    private void SyncReportType()
    {
        var hasChart = blocks.Any(b => b.BlockType == "Chart");
        var hasTable = blocks.Any(b => b.BlockType == "Table");
        Model.Data.Type = hasChart && hasTable ? "Combination" : hasChart ? "Chart" : "Table";
    }

    private static string GetBlockTypeName(string blockType) => blockType == "Chart" ? "图表" : "表格";

    private void OnAddChartBlock()
    {
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = "Chart",
            Title = $"图表 {blocks.Count + 1}",
            Width = 12,
            Chart = new ChartConfig { Title = $"图表 {blocks.Count + 1}", DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
        SyncReportType();
        StateChanged();
    }

    private void OnAddTableBlock()
    {
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = "Table",
            Title = $"表格 {blocks.Count + 1}",
            Width = 12,
            Table = new TableConfig { Columns = [], DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
        SyncReportType();
        StateChanged();
    }

    private void OnSelectBlock(ReportBlock block)
    {
        selectedBlock = block;
        StateChanged();
    }

    private void OnRemoveBlock(ReportBlock block)
    {
        blocks.Remove(block);
        SyncReportType();
        if (selectedBlock?.Id == block.Id)
            selectedBlock = blocks.FirstOrDefault();
        StateChanged();
    }

    private DropdownModel GetBlockMenu(ReportBlock block)
    {
        return new DropdownModel
        {
            Icon = "ellipsis",
            TriggerType = "Click",
            Items =
            [
                new ActionInfo("删除") { OnClick = this.Callback<MouseEventArgs>(e => OnRemoveBlock(block)) }
            ]
        };
    }

    private void OnAddColumn()
    {
        selectedBlock?.Table?.Columns?.Add(new ColumnConfig());
        StateChanged();
    }

    private void OnDeleteColumn(ColumnConfig column)
    {
        selectedBlock?.Table?.Columns?.Remove(column);
        StateChanged();
    }
}
