namespace Known.Razor.Components;

class AdminTenant : BaseComponent
{
    private SysTenant tenant;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Span("tenantName", $"当前租户：{tenant?.Name}");
        builder.Link("切换", Callback(OnChangeTenant));
    }

    private void OnChangeTenant()
    {
        UI.Show<AdminTenantGrid>("切换租户", new Size(600, 400), action: attr =>
        {
            attr.Set(c => c.OnPicked, OnPicked);
        });
    }

    private async void OnPicked(object row)
    {
        tenant = row as SysTenant;
        var result = await Platform.System.ChangeTenantAsync(tenant);
        if (!result.IsValid)
        {
            UI.Alert(result.Message);
            return;
        }
        StateChanged();
    }
}

class AdminTenantGrid : DataGrid<SysTenant>
{
    public AdminTenantGrid()
    {
        var builder = new ColumnBuilder<SysTenant>();
        builder.Field(c => c.Code);
        builder.Field(c => c.Name, true);
        builder.Field(c => c.Note);
        Columns = builder.ToColumns();
    }

    protected override Task<PagingResult<SysTenant>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.System.QueryTenantsAsync(criteria);
    }

    public override void OnRowDoubleClick(int row, SysTenant item)
    {
        OnPicked?.Invoke(item);
        UI.CloseDialog();
    }
}