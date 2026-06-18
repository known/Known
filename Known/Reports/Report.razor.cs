namespace Known.Reports;

/// <summary>
/// 报表中心页面组件类。
/// </summary>
public partial class Report
{
    private IReportService Service;
    private List<SysReport> reports = [];
    private List<CodeInfo> items = [];
    private SysReport currentReport;
    private ReportType reportType;
    private ReportConfig reportConfig;
    private ChartConfig chartConfig;
    private TableConfig tableConfig;
    private KChart chart;
    private TableModel<Dictionary<string, object>> tableModel;
    private ReportForm settingRef;
    private bool needsChartRefresh;

    /// <summary>
    /// 取得系统ID。
    /// </summary>
    public virtual string SysId { get; } = Config.App.Id;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IReportService>();
        tableModel = new TableModel<Dictionary<string, object>>(this)
        {
            IsAutoLoad = false,
            ShowPager = false,
            ShowSetting = false,
            FixedHeight = "400px"
        };
        await LoadReportsAsync();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            if (currentReport != null)
                await ShowReportAsync(currentReport);
        }

        if (needsChartRefresh)
        {
            needsChartRefresh = false;
            await ShowChartAsync();
        }
    }

    private async Task LoadReportsAsync()
    {
        var reports = await Service.GetReportsAsync(SysId);
        items = [.. reports.Select(r => new CodeInfo(r.Id, r.Name))];
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
        var result = await Service.DeleteReportAsync(item);
        if (result.IsValid)
        {
            await LoadReportsAsync();
            if (currentReport?.Id == item.Id)
            {
                currentReport = null;
                reportType = ReportType.Table;
                chartConfig = null;
                StateChanged();
            }
        }
    }

    private void ShowForm(SysReport item)
    {
        var model = new FormModel<SysReport>(this)
        {
            Title = item.IsNew ? "新建报表" : "编辑报表",
            Type = typeof(ReportForm),
            Info = new FormInfo { Width = 1000 },
            Data = item,
            OnSave = Service.SaveReportAsync,
            OnSaved = async d => await LoadReportsAsync()
        };
        UI.ShowForm(model);
    }

    private async Task ShowReportAsync(SysReport report)
    {
        if (report == null)
            return;

        reportConfig = !string.IsNullOrWhiteSpace(report.Config)
            ? Utils.FromJson<ReportConfig>(report.Config) ?? new ReportConfig()
            : new ReportConfig();

        reportType = (report.Type ?? "Table") switch
        {
            "Table" => ReportType.Table,
            "Chart" => ReportType.Chart,
            "Combination" => ReportType.Combination,
            _ => ReportType.Table
        };

        chartConfig = reportConfig.Charts?.FirstOrDefault();
        tableConfig = reportConfig.Tables?.FirstOrDefault();

        if (reportType == ReportType.Table || reportType == ReportType.Combination)
            SetupTable();

        if (reportType == ReportType.Chart || reportType == ReportType.Combination)
            needsChartRefresh = true;

        StateChanged();
    }

    private void SetupTable()
    {
        tableModel.Clear();

        if (tableConfig?.Columns != null)
        {
            foreach (var col in tableConfig.Columns)
            {
                tableModel.Columns.Add(new ColumnInfo
                {
                    Id = col.Field,
                    Name = col.Title,
                    Width = col.Width,
                    Align = col.Align
                });
            }
        }
    }

    private async Task ShowChartAsync()
    {
        if (chart == null || chartConfig == null)
            return;

        var data = new List<ChartDataInfo>();
        if (chartConfig.ChartType == "bar")
            await chart.ShowBarAsync(chartConfig.Title, [.. data]);
        else if (chartConfig.ChartType == "line")
            await chart.ShowLineAsync(chartConfig.Title, [.. data]);
    }

    private DropdownModel GetDropdownModel(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        return new DropdownModel
        {
            Icon = "menu",
            TriggerType = "Click",
            Tooltip = Language.Action,
            Items =
            [
                new ActionInfo(Language.Edit) { OnClick = this.Callback<MouseEventArgs>(e => OnEditClick(report)) },
                new ActionInfo(Language.Delete) { OnClick = this.Callback<MouseEventArgs>(e => OnDeleteClick(report)) }
            ]
        };
    }
}