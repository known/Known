namespace Known.Reports;

/// <summary>
/// 报表视图组件。
/// </summary>
public partial class ReportView
{
    private IReportService Service;
    private int gridColumns = 3;
    private readonly Dictionary<string, KChart> chartRefs = [];

    private SysReport Report { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IReportService>();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
    }

    /// <summary>
    /// 显示报表布局和数据。
    /// </summary>
    /// <param name="report">报表信息。</param>
    public async Task ShowReportAsync(SysReport report)
    {
        if (report == null)
            return;

        Report = report;
        gridColumns = Math.Clamp(report.GridColumn ?? 1, 1, 6);

        foreach (var block in report.Blocks)
        {
            var data = await Service.QueryBlockDataAsync(block);
            if (data == null || data.Count == 0)
                continue;

            if (block.BlockType == ReportBlockType.Chart)
            {
                await ShowChartAsync(block, data);
            }
            else
            {
                var model = new TableModel<Dictionary<string, object>>(this)
                {
                    IsAutoLoad = false,
                    ShowPager = false,
                    ShowSetting = false,
                    FixedHeight = "400px",
                    DataSource = [.. data]
                };
                SetupTable(model, block);
                block.Table = model;
            }
        }

        StateChanged();
    }

    private string GetGridStyle()
    {
        return $"grid-template-columns: repeat({gridColumns}, 1fr);";
    }

    private async Task ShowChartAsync(ReportBlock block, List<Dictionary<string, object>> data)
    {
        if (!chartRefs.TryGetValue(block.Id, out var chart))
            return;

        var chartConfig = block.Chart;
        if (chartConfig == null)
            return;

        var xField = chartConfig.XField;
        var yField = chartConfig.YField;
        var categoryField = chartConfig.CategoryField;

        if (string.IsNullOrWhiteSpace(xField) || string.IsNullOrWhiteSpace(yField))
            return;

        if (!string.IsNullOrWhiteSpace(categoryField))
        {
            var groups = data.GroupBy(d => d.GetValue<string>(categoryField) ?? "");
            var chartDatas = groups.Select(g =>
            {
                var values = data.Where(d => (d.GetValue<string>(categoryField) ?? "") == g.Key)
                                 .ToDictionary(d => d.GetValue<string>(xField) ?? "", d => d.GetValue<object>(yField));
                return new ChartDataInfo { Name = g.Key, Series = values };
            }).ToArray();

            if (chartConfig.ChartType == ChartType.Bar)
                await chart.ShowBarAsync(block.Title, chartDatas);
            else if (chartConfig.ChartType == ChartType.Line)
                await chart.ShowLineAsync(block.Title, chartDatas);
            //else if (chartConfig.ChartType == ChartType.Pie)
            //    await chart.ShowBarAsync(block.Title, chartDatas);
        }
        else
        {
            var chartData = new ChartDataInfo
            {
                Name = yField,
                Series = data.ToDictionary(d => d.GetValue<string>(xField) ?? "", d => d.GetValue<object>(yField))
            };

            if (chartConfig.ChartType == ChartType.Bar)
                await chart.ShowBarAsync(block.Title, [chartData]);
            else if (chartConfig.ChartType == ChartType.Line)
                await chart.ShowLineAsync(block.Title, [chartData]);
            //else if (chartConfig.ChartType == ChartType.Pie)
            //    await chart.ShowPieAsync(block.Title, [chartData]);
        }
    }

    private void SetupTable(TableModel<Dictionary<string, object>> model, ReportBlock block)
    {
        model.Clear();

        var hasColumns = block?.Columns != null && block.Columns.Count > 0;
        if (hasColumns)
        {
            foreach (var col in block.Columns)
            {
                model.Columns.Add(new ColumnInfo
                {
                    Id = col.Field,
                    Name = col.Title,
                    Width = col.Width,
                    Align = col.Align.ToString().ToLower()
                });
            }
        }
        else if (block?.DataSource?.SourceType == DataSourceType.Entity && block.DataSource.EntityFields?.Count > 0)
        {
            foreach (var ef in block.DataSource.EntityFields)
            {
                var aggSuffix = ef.AggregateType != AggregateType.None ? $"({ef.AggregateType})" : "";
                model.Columns.Add(new ColumnInfo
                {
                    Id = ef.DisplayName ?? ef.FieldName,
                    Name = $"{ef.DisplayName ?? ef.FieldName}{aggSuffix}"
                });
            }
        }
    }
}