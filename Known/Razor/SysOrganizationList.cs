using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private List<SysOrganization> datas;
    private MenuItem parent;

    protected override async Task OnInitPageAsync()
    {
        datas = await Platform.Company.GetOrganizationsAsync();
        await base.OnInitPageAsync();
        Page.FormTitle = row => $"{Name} - {row.ParentName}";
        Page.Tree = new TreeModel
        {
            ExpandParent = true,
            Data = datas.ToMenuItems(),
            OnNodeClick = n => parent = n
        };
        Page.Table.ShowPager = false;
    }

    public void New()
    {
        if (parent == null)
        {
            UI.Error("请先选择上级组织！");
            return;
        }

        Page.NewForm(Platform.Company.SaveOrganizationAsync, new SysOrganization { ParentId = parent.Id, ParentName = parent.Name });
    }

    public void Edit(SysOrganization row) => Page.EditForm(Platform.Company.SaveOrganizationAsync, row);
    public void Delete(SysOrganization row) => Page.Delete(Platform.Company.DeleteOrganizationsAsync, row);
    public void DeleteM() => Page.DeleteM(Platform.Company.DeleteOrganizationsAsync);
}