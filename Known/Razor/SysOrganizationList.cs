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
            OnNodeClick = OnNodeClick,
            OnRefresh = OnTreeRefresh
        };
        Page.Table.ShowPager = false;

        if (datas != null && datas.Count > 0)
        {
            parent = Page.Tree.Data[0];
            Page.Tree.SelectedKeys = [parent.Id];
        }
    }

	protected override Task<PagingResult<SysOrganization>> OnQueryAsync(PagingCriteria criteria)
	{
		var data = parent?.Children?.Select(c => (SysOrganization)c.Data).ToList();
		var result = new PagingResult<SysOrganization> { PageData = data, TotalCount = data?.Count ?? 0 };
		return Task.FromResult(result);
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

	private async void OnNodeClick(MenuItem item)
	{
		parent = item;
        await Page.Table.RefreshAsync();
	}

	private async Task OnTreeRefresh()
	{
		datas = await Platform.Company.GetOrganizationsAsync();
		Page.Tree.Data = datas.ToMenuItems();
	}
}