using Known.Entities;
using Known.Extensions;

namespace Known.Blazor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private MenuItem current;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Page.FormTitle = row => $"{Name} - {row.ParentName}";
        Page.Tree = new TreeModel
        {
            ExpandParent = true,
            OnNodeClick = OnNodeClick,
            OnRefresh = OnTreeRefresh
        };
        Page.Table.RowKey = r => r.Id;
        Page.Table.ShowPager = false;

        var datas = await Platform.Company.GetOrganizationsAsync();
        if (datas != null && datas.Count > 0)
        {
            Page.Tree.Data = datas.ToMenuItems(ref current);
            current = Page.Tree.Data[0];
            Page.Tree.SelectedKeys = [current.Id];
        }
    }

    protected override Task<PagingResult<SysOrganization>> OnQueryAsync(PagingCriteria criteria)
    {
        var data = current?.Children?.Select(c => (SysOrganization)c.Data).ToList();
        var result = new PagingResult<SysOrganization>(data);
        return Task.FromResult(result);
    }

    [Action]
    public void New()
    {
        if (current == null)
        {
            UI.Error("请先选择上级组织！");
            return;
        }

        Page.NewForm(Platform.Company.SaveOrganizationAsync, new SysOrganization { ParentId = current?.Id, ParentName = current?.Name });
    }

    [Action] public void Edit(SysOrganization row) => Page.EditForm(Platform.Company.SaveOrganizationAsync, row);
    [Action] public void Delete(SysOrganization row) => Page.Delete(Platform.Company.DeleteOrganizationsAsync, row);
    [Action] public void DeleteM() => Page.DeleteM(Platform.Company.DeleteOrganizationsAsync);

    private async void OnNodeClick(MenuItem item)
    {
        current = item;
        await Page.Table.RefreshAsync();
    }

    private async Task OnTreeRefresh()
    {
        var datas = await Platform.Company.GetOrganizationsAsync();
        Page.Tree.Data = datas.ToMenuItems(ref current);
    }
}