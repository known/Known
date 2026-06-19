namespace Known.Reports;

/// <summary>
/// 报表页面组件。
/// </summary>
public partial class Report
{
    private IReportService Service;
    private List<SysReport> reports = [];
    private List<CodeInfo> items = [];
    private SysReport currentReport;
    private List<ReportBlock> blocks = [];
    private int gridColumns = 3;
    private bool needLoadData;
    private KListBox listBox;
    private readonly List<KChart> chartRefs = [];
    private readonly Dictionary<string, TableModel<Dictionary<string, object>>> tableModels = [];

    /// <summary>
    /// 取得子系统ID。
    /// </summary>
    public virtual string SysId { get; } = Config.App.Id;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IReportService>();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            await LoadReportsAsync();
        }
        if (needLoadData)
        {
            needLoadData = false;
            await LoadBlockDataAsync();
        }
    }

    private async Task LoadReportsAsync()
    {
        reports = await Service.GetReportsAsync(SysId);
        items = [.. reports.Select(r => new CodeInfo(r.IsFixed ? "System" : "", r.Id, r.Name, null))];
        if (currentReport != null)
            currentReport = reports.FirstOrDefault(r => r.Id == currentReport.Id);
        currentReport ??= reports.FirstOrDefault();
        listBox?.SetListBox(items, currentReport?.Id);
        await ShowReportAsync(currentReport);
    }

    private async Task OnReportClick(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        if (report != null)
        {
            currentReport = report;
            await ShowReportAsync(report);
        }
    }

    private void OnAddClick() => ShowForm(new SysReport { SysId = SysId });
    private void OnEditClick(SysReport item) => ShowForm(item);

    private async Task OnDeleteClick(SysReport item)
    {
        UI.Confirm("确定要删除该报表吗？", async () =>
        {
            var result = await Service.DeleteReportAsync(item);
            if (result.IsValid)
            {
                await LoadReportsAsync();
                if (currentReport?.Id == item.Id)
                {
                    currentReport = null;
                    blocks = [];
                    StateChanged();
                }
            }
        });
    }

    private void ShowForm(SysReport item)
    {
        var model = new FormModel<SysReport>(this)
        {
            Title = item.IsNew ? "新增报表" : "编辑报表",
            Type = typeof(ReportForm),
            Info = new FormInfo { Width = 1100 },
            Data = item,
            OnSave = Service.SaveReportAsync,
            OnSaved = async d =>
            {
                await LoadReportsAsync();
                if (d != null)
                {
                    currentReport = reports.FirstOrDefault(r => r.Id == d.Id);
                    if (currentReport != null)
                        await ShowReportAsync(currentReport);
                }
            }
        };
        UI.ShowForm(model);
    }

    private async Task ShowReportAsync(SysReport report)
    {
        if (report == null)
            return;

        blocks = report.Blocks ?? [];
        gridColumns = Math.Clamp(report.GridColumn ?? 1, 1, 6);

        chartRefs.Clear();
        tableModels.Clear();
        for (int i = 0; i < blocks.Count; i++)
            chartRefs.Add(null);

        foreach (var block in blocks.Where(b => b.BlockType == ReportBlockType.Table))
        {
            var model = new TableModel<Dictionary<string, object>>(this)
            {
                IsAutoLoad = false,
                ShowPager = false,
                ShowSetting = false,
                FixedHeight = "400px"
            };
            SetupTable(model, block);
            tableModels[block.Id] = model;
        }

        needLoadData = true;
        StateChanged();
    }

    private async Task LoadBlockDataAsync()
    {
        var hasData = false;
        foreach (var block in blocks)
        {
            var data = await Service.QueryBlockDataAsync(block);
            if (data == null || data.Count == 0)
                continue;

            hasData = true;
            if (block.BlockType == ReportBlockType.Chart)
            {
                await ShowChartAsync(block, data);
            }
            else
            {
                var model = GetTableModel(block);
                if (model != null)
                {
                    model.DataSource = [.. data];
                }
            }
        }
        if (hasData)
            StateChanged();
    }

    private async Task ShowChartAsync(ReportBlock block, List<Dictionary<string, object>> data)
    {
        var chart = chartRefs[blocks.IndexOf(block)];
        if (chart == null)
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

            if (chartConfig.ChartType == ChartType.Line)
                await chart.ShowLineAsync(block.Title, chartDatas);
            else
                await chart.ShowBarAsync(block.Title, chartDatas);
        }
        else
        {
            var chartData = new ChartDataInfo
            {
                Name = yField,
                Series = data.ToDictionary(d => d.GetValue<string>(xField) ?? "", d => d.GetValue<object>(yField))
            };

            if (chartConfig.ChartType == ChartType.Line)
                await chart.ShowLineAsync(block.Title, [chartData]);
            else
                await chart.ShowBarAsync(block.Title, [chartData]);
        }
    }

    private string GetGridStyle()
    {
        return $"grid-template-columns: repeat({gridColumns}, 1fr);";
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

    private TableModel<Dictionary<string, object>> GetTableModel(ReportBlock block)
    {
        return tableModels.TryGetValue(block.Id, out var model) ? model : null;
    }

    private DropdownModel GetDropdownModel(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        if (report == null)
            return null;

        var menuItems = new List<ActionInfo>();

        if (!report.IsFixed || CurrentUser.IsSystemAdmin())
        {
            menuItems.Add(new ActionInfo(Language.Edit)
            {
                OnClick = this.Callback<MouseEventArgs>(e => OnEditClick(report))
            });
            menuItems.Add(new ActionInfo(Language.Delete)
            {
                OnClick = this.Callback<MouseEventArgs>(e => OnDeleteClick(report))
            });
        }

        return menuItems.Count > 0 ? new DropdownModel
        {
            Icon = "menu",
            TriggerType = "Click",
            Tooltip = Language.Action,
            Items = menuItems
        } : null;
    }
}
