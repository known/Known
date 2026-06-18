namespace Known.Reports;

/// <summary>
/// 报表中心页面组件类。
/// </summary>
public partial class Report
{
    private const string ReportBizType = "Report";
    private string searchKey;
    private List<ReportInfo> reports = [];
    private List<ReportInfo> filteredReports = [];
    private List<CodeInfo> items = [];
    private ReportInfo currentReport;
    private ReportDisplayType reportType;
    private ReportConfig reportConfig;
    private ChartConfig chartConfig;
    private TableConfig tableConfig;
    private KChart chart;
    private TableModel<Dictionary<string, object>> tableModel;
    private ReportSetting settingRef;
    private bool needsChartRefresh;

    /// <summary>
    /// 取得系统ID。
    /// </summary>
    public virtual string SysId { get; } = Config.App.Id;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
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
        var json = await Admin.GetUserSettingAsync(ReportBizType);
        if (!string.IsNullOrWhiteSpace(json))
            reports = Utils.FromJson<List<ReportInfo>>(json) ?? [];
        else
            reports = GetSampleReports();

        items = [.. reports.Select(r => new CodeInfo(r.Id, r.Name))];
        ApplySearch();
    }

    private async Task SaveReportsAsync()
    {
        await Admin.SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = ReportBizType,
            BizData = Utils.ToJson(reports)
        });
    }

    private static List<ReportInfo> GetSampleReports()
    {
        var config1 = new ReportConfig
        {
            Chart = new ChartConfig { ChartType = "bar", Title = "月度销售统计", XField = "month", YField = "amount" },
            Table = new TableConfig
            {
                Columns =
                [
                    new ColumnConfig { Field = "month", Title = "月份", Width = 120 },
                    new ColumnConfig { Field = "amount", Title = "销售额", Width = 120, Type = "number", Align = "right" }
                ]
            }
        };
        var config2 = new ReportConfig
        {
            Table = new TableConfig
            {
                Columns =
                [
                    new ColumnConfig { Field = "name", Title = "产品名称", Width = 150 },
                    new ColumnConfig { Field = "category", Title = "分类", Width = 100 },
                    new ColumnConfig { Field = "price", Title = "单价", Width = 100, Type = "number", Align = "right" },
                    new ColumnConfig { Field = "stock", Title = "库存", Width = 80, Type = "number", Align = "right" }
                ]
            }
        };
        var config3 = new ReportConfig
        {
            Chart = new ChartConfig { ChartType = "line", Title = "年度趋势分析", XField = "month", YField = "value" }
        };

        return
        [
            new() { Name = "月度销售统计", Type = "Combination", Config = Utils.ToJson(config1), Note = "每月销售数据统计报表" },
            new() { Name = "产品库存清单", Type = "Table", Config = Utils.ToJson(config2), Note = "产品库存明细列表" },
            new() { Name = "年度趋势分析", Type = "Chart", Config = Utils.ToJson(config3), Note = "全年数据趋势" }
        ];
    }

    private void ApplySearch()
    {
        if (string.IsNullOrWhiteSpace(searchKey))
            filteredReports = [.. reports];
        else
            filteredReports = reports.Where(r => r.Name.Contains(searchKey)).ToList();
        StateChanged();
    }

    private Task OnSearchAsync(string key)
    {
        searchKey = key;
        ApplySearch();
        return Task.CompletedTask;
    }

    private void OnAddClick()
    {
        settingRef = null;
        DialogModel model = null;
        model = new DialogModel
        {
            Title = "新增报表",
            Width = 800,
            Maximizable = true,
            Content = builder =>
            {
                builder.Component<ReportSetting>()
                       .Set(c => c.Report, new ReportInfo())
                       .Set(c => c.OnSave, OnSaveReport)
                       .Build(value => settingRef = value);
            },
            OnOk = async () =>
            {
                if (settingRef == null)
                    return;

                settingRef.SaveConfig();
                var result = await OnSaveReport(settingRef.Report);
                if (result.IsValid)
                {
                    await model.CloseAsync();
                    await LoadReportsAsync();
                }
                else
                {
                    UI.Error(result.Message);
                }
            }
        };
        UI.ShowDialog(model);
    }

    private void OnEditClick(ReportInfo report)
    {
        settingRef = null;
        DialogModel model = null;
        model = new DialogModel
        {
            Title = "编辑报表",
            Width = 800,
            Maximizable = true,
            Content = builder =>
            {
                builder.Component<ReportSetting>()
                       .Set(c => c.Report, report)
                       .Set(c => c.OnSave, OnSaveReport)
                       .Build(value => settingRef = value);
            },
            OnOk = async () =>
            {
                if (settingRef == null)
                    return;

                settingRef.SaveConfig();
                var result = await OnSaveReport(settingRef.Report);
                if (result.IsValid)
                {
                    await model.CloseAsync();
                    await LoadReportsAsync();
                    if (currentReport?.Id == report.Id)
                    {
                        currentReport = reports.FirstOrDefault(r => r.Id == report.Id);
                        await ShowReportAsync(currentReport);
                    }
                }
                else
                {
                    UI.Error(result.Message);
                }
            }
        };
        UI.ShowDialog(model);
    }

    private async Task<Result> OnSaveReport(ReportInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.Name))
            return Result.Error("请输入报表名称");

        if (string.IsNullOrWhiteSpace(info.Config))
        {
            var config = new ReportConfig
            {
                Chart = new ChartConfig { ChartType = "bar", Title = info.Name },
                Table = new TableConfig()
            };
            info.Config = Utils.ToJson(config);
        }

        var existing = reports.FirstOrDefault(r => r.Id == info.Id);
        if (existing != null)
        {
            existing.Name = info.Name;
            existing.Type = info.Type;
            existing.Config = info.Config;
            existing.Note = info.Note;
        }
        else
        {
            reports.Add(info);
        }

        await SaveReportsAsync();
        currentReport = reports.FirstOrDefault(r => r.Id == info.Id);
        await ShowReportAsync(currentReport);
        return Result.Success("保存成功");
    }

    private async Task OnDeleteClick(ReportInfo report)
    {
        reports.Remove(report);
        await SaveReportsAsync();
        if (currentReport?.Id == report.Id)
        {
            currentReport = null;
            reportType = ReportDisplayType.Table;
            chartConfig = null;
        }
        ApplySearch();
    }

    private async Task OnReportClick(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        currentReport = report;
        ApplySearch();
        await ShowReportAsync(report);
    }

    private async Task ShowReportAsync(ReportInfo report)
    {
        if (report == null)
            return;

        reportConfig = !string.IsNullOrWhiteSpace(report.Config)
            ? Utils.FromJson<ReportConfig>(report.Config) ?? new ReportConfig()
            : new ReportConfig();

        reportType = report.Type switch
        {
            "Table" => ReportDisplayType.Table,
            "Chart" => ReportDisplayType.Chart,
            "Combination" => ReportDisplayType.Combination,
            _ => ReportDisplayType.Table
        };

        chartConfig = reportConfig.Chart;
        tableConfig = reportConfig.Table;

        if (reportType == ReportDisplayType.Table || reportType == ReportDisplayType.Combination)
            SetupTable();

        if (reportType == ReportDisplayType.Chart || reportType == ReportDisplayType.Combination)
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

        tableModel.DataSource = GetSampleTableData();
    }

    private static List<Dictionary<string, object>> GetSampleTableData()
    {
        return
        [
            new() { ["name"] = "笔记本电脑", ["category"] = "电子产品", ["price"] = 5999, ["stock"] = 120 },
            new() { ["name"] = "机械键盘", ["category"] = "外设", ["price"] = 899, ["stock"] = 350 },
            new() { ["name"] = "无线鼠标", ["category"] = "外设", ["price"] = 199, ["stock"] = 500 },
            new() { ["name"] = "4K显示器", ["category"] = "电子产品", ["price"] = 2999, ["stock"] = 80 },
            new() { ["name"] = "移动硬盘", ["category"] = "存储", ["price"] = 499, ["stock"] = 200 },
            new() { ["name"] = "内存条 16GB", ["category"] = "配件", ["price"] = 399, ["stock"] = 600 }
        ];
    }

    private async Task ShowChartAsync()
    {
        if (chart == null || chartConfig == null)
            return;

        var data = GetSampleChartData();
        if (chartConfig.ChartType == "bar")
            await chart.ShowBarAsync(chartConfig.Title, data);
        else if (chartConfig.ChartType == "line")
            await chart.ShowLineAsync(chartConfig.Title, data);
    }

    private ChartDataInfo[] GetSampleChartData()
    {
        var months = new[] { "一月", "二月", "三月", "四月", "五月", "六月" };
        var rand = new Random();
        var fields = string.IsNullOrWhiteSpace(chartConfig?.YField)
            ? ["amount"]
            : chartConfig.YField.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return fields.Select(field =>
        {
            var series = new Dictionary<string, object>();
            foreach (var month in months)
                series[month] = rand.Next(1000, 10000);
            return new ChartDataInfo { Name = field, Series = series };
        }).ToArray();
    }

    private DropdownModel GetDropdownModel(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        return new DropdownModel
        {
            Icon = "menu",
            TriggerType = "Click",
            Tooltip = "操作",
            Items =
            [
                new ActionInfo(Language.Edit) { OnClick = this.Callback<MouseEventArgs>(e => OnEditClick(report)) },
                new ActionInfo(Language.Delete) { OnClick = this.Callback<MouseEventArgs>(e => OnDeleteClick(report)) }
            ]
        };
    }
}
