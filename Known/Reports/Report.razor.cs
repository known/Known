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
    private readonly List<KChart?> chartRefs = [];
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
        await LoadReportsAsync();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender && currentReport != null)
            await ShowReportAsync(currentReport);
    }

    private async Task LoadReportsAsync()
    {
        reports = await Service.GetReportsAsync(SysId);
        items = [.. reports.Select(r => new CodeInfo(
            r.IsFixed ? "System" : "",
            r.Id,
            r.Name,
            null
        ))];
        StateChanged();
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
                blocks = [];
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
            Info = new FormInfo { Width = 1100 },
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

        var config = !string.IsNullOrWhiteSpace(report.Config)
            ? Utils.FromJson<ReportConfig>(report.Config) ?? new ReportConfig()
            : new ReportConfig();

        blocks = config.Blocks ?? [];

        if (blocks.Count == 0)
        {
            if (config.Charts?.Count > 0)
            {
                foreach (var chart in config.Charts)
                    blocks.Add(new ReportBlock
                    {
                        BlockType = "Chart",
                        Title = chart.Title,
                        Width = 12,
                        Chart = chart,
                        DataSource = chart.DataSource ?? new DataSourceConfig()
                    });
            }
            if (config.Tables?.Count > 0)
            {
                foreach (var table in config.Tables)
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

        chartRefs.Clear();
        tableModels.Clear();
        for (int i = 0; i < blocks.Count; i++)
            chartRefs.Add(null);

        foreach (var block in blocks.Where(b => b.BlockType == "Table"))
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

        StateChanged();
    }

    private void SetupTable(TableModel<Dictionary<string, object>> model, ReportBlock block)
    {
        model.Clear();

        if (block?.Table?.Columns != null)
        {
            foreach (var col in block.Table.Columns)
            {
                model.Columns.Add(new ColumnInfo
                {
                    Id = col.Field,
                    Name = col.Title,
                    Width = col.Width,
                    Align = col.Align
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

        if (!report.IsFixed)
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
