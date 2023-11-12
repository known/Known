using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private List<SysOrganization> datas;

    protected override async Task OnInitPageAsync()
    {
        datas = await Platform.Company.GetOrganizationsAsync();
        await base.OnInitPageAsync();
        Tree = new TreeModel
        {
            Data = datas.ToMenuItems()
        };
    }

    public void New() => Table.NewForm(Platform.Company.SaveOrganizationAsync, new SysOrganization());
    public void Edit(SysOrganization row) => Table.EditForm(Platform.Company.SaveOrganizationAsync, row);
    public void Delete(SysOrganization row) => Table.Delete(Platform.Company.DeleteOrganizationsAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.Company.DeleteOrganizationsAsync);
}