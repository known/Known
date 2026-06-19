namespace Known.Reports;

/// <summary>
/// 报表页面组件。
/// </summary>
public partial class Report
{
    private IReportService Service;
    private List<SysReport> reports = [];
    private List<CodeInfo> items = [];
    private SysReport current;
    private KListBox listBox;
    private ReportView view;

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
    }

    private async Task LoadReportsAsync()
    {
        reports = await Service.GetReportsAsync(SysId);
        items = [.. reports.Select(r => new CodeInfo(r.IsFixed ? "System" : "", r.Id, r.Name, null))];
        if (current != null)
            current = reports.FirstOrDefault(r => r.Id == current.Id);
        current ??= reports.FirstOrDefault();
        listBox?.SetListBox(items, current?.Id);
        await view?.ShowReportAsync(current);
    }

    private async Task OnReportClick(CodeInfo item)
    {
        var report = reports.FirstOrDefault(r => r.Id == item.Code);
        if (report != null)
        {
            current = report;
            await view?.ShowReportAsync(current);
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