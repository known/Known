namespace Known.Reports;

/// <summary>
/// 报表页面组件。
/// </summary>
public partial class Report
{
    private IReportService Service;
    private readonly List<CodeInfo> items = [];
    private List<SysReport> reports = [];
    private CodeInfo currentItem;
    private SysReport current;
    private KListBox listBox;
    private ReportView view;

    /// <summary>
    /// 取得子系统ID。
    /// </summary>
    public virtual string SysId { get; } = Config.App.Id;

    /// <summary>
    /// 取得是否允许添加报表，默认允许添加，子类可重写以禁用添加功能。
    /// </summary>
    public virtual bool IsAdd { get; } = true;

    /// <summary>
    /// 取得报表组件列表。
    /// </summary>
    public List<ComponentInfo> Reports { get; } = [];

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
    }

    private async Task LoadReportsAsync()
    {
        items.Clear();
        foreach (var item in Reports)
        {
            items.Add(new CodeInfo("System", $"{item.Id}", item.Name, item));
        }
        reports = await Service.GetReportsAsync(SysId);
        foreach (var item in reports)
        {
            items.Add(new CodeInfo(item.IsFixed ? "Fixed" : "", item.Id, item.Name, null));
        }
        if (current != null)
            current = reports.FirstOrDefault(r => r.Id == current.Id);
        current ??= reports.FirstOrDefault();
        listBox?.SetDataSource(items, current?.Id);
        await ShowReportAsync(current);
    }

    private async Task OnReportClick(CodeInfo item)
    {
        currentItem = item;
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        if (report != null)
        {
            current = report;
            await ShowReportAsync(current);
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
                if (current?.Id == item.Id)
                {
                    current = null;
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
            OnSaved = async d => await LoadReportsAsync()
        };
        UI.ShowForm(model);
    }

    private async Task ShowReportAsync(SysReport current)
    {
        if (view != null)
            await view.ShowReportAsync(current);
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