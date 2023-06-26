using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysTenantList : DataGrid<SysTenant, SysTenantForm>
{
    protected override Task<PagingResult<SysTenant>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.System.QueryTenantsAsync(criteria);
    }

    public void New() => ShowForm(new SysTenant { Enabled = true, UserCount = 1, BillCount = 100 });
    public void Edit(SysTenant row) => ShowForm(row);
}