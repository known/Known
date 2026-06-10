namespace Known.Internals;

class TenantSwitch : BaseTable<SysCompany>
{
    private ICompanyService Service;

    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    [Parameter] public Action OnChange { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ICompanyService>();

        Table.Name = Language.SelectTenant;
        Table.PageSize = 10;
        Table.FixedHeight = "300px";
        Table.OnQuery = QueryTenantsAsync;

        Table.AddColumn(c => c.Code).Width(120).ViewLink(false);
        Table.AddColumn(c => c.Name, true).Width(200);
        Table.AddColumn(c => c.Industry).Width(120);
        Table.AddAction(nameof(Switch), "切换");
    }

    [Action]
    public void Switch(SysCompany row)
    {
        if (row == null || row.Code == CurrentUser?.CompNo)
            return;

        UI.Confirm($"确定要切换到租户【{row.Name}】吗？", async () =>
        {
            var result = await Service.SwitchTenantAsync(row);
            UI.Result(result, async () =>
            {
                Context.CurrentUser.IsChangeTenant = true;
                Context.CurrentUser.CompNo = row.Code;
                Context.CurrentUser.CompName = row.Name;
                await AuthProvider?.SignInAsync(Context.CurrentUser);
                OnChange?.Invoke();
                Navigation.Refresh();
            });
        });
    }

    private Task<PagingResult<SysCompany>> QueryTenantsAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysCompany.Manager), QueryType.Equal, CurrentUser.UserName);
        return Service.QueryTenantsAsync(criteria);
    }
}