namespace Known.Reports;

/// <summary>
/// 报表设置表单类。
/// </summary>
public partial class ReportForm
{
    private ReportConfig reportConfig;
    private List<ReportBlock> blocks = [];
    private ReportBlock selectedBlock;

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
        selectedBlock = blocks.FirstOrDefault();
    }

    private void OnAddChartBlock()
    {
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = ReportBlockType.Chart,
            Title = $"图表 {blocks.Count + 1}",
            Width = 12,
            Chart = new ChartConfig { Title = $"图表 {blocks.Count + 1}", DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
        StateChanged();
    }

    private void OnAddTableBlock()
    {
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = ReportBlockType.Table,
            Title = $"表格 {blocks.Count + 1}",
            Width = 12,
            Table = new TableConfig { Columns = [], DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
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
                new ActionInfo(Language.Delete) { OnClick = this.Callback<MouseEventArgs>(e => OnRemoveBlock(block)) }
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