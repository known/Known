using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private MenuItem current;
    private TreeModel tree;
    private TablePageModel<SysOrganization> table;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();

		Page.Type = PageType.Column;
		Page.Spans = [4, 20];
		Page.Contents = [BuildTree, BuildTable];

		tree = new TreeModel
		{
			ExpandParent = true,
			OnNodeClick = OnNodeClick,
			OnRefresh = OnTreeRefresh
		};

		table = new TablePageModel<SysOrganization>(this)
		{
			FormTitle = row => $"{Name} - {row.ParentName}",
			RowKey = r => r.Id,
			ShowPager = false,
			OnQuery = OnQueryOrganizationsAsync,
			OnToolClick = OnToolClick,
			OnAction = OnActionClick
		};

		var datas = await Platform.Company.GetOrganizationsAsync();
		if (datas != null && datas.Count > 0)
		{
			tree.Data = datas.ToMenuItems(ref current);
			current = tree.Data[0];
			tree.SelectedKeys = [current.Id];
		}
	}

	public override async Task RefreshAsync()
	{
		await tree.RefreshAsync();
		//model.StateChanged.Invoke();
		await table.RefreshAsync();
		StateChanged();
	}

	private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => UI.BuildPage(builder, table);

	private Task<PagingResult<SysOrganization>> OnQueryOrganizationsAsync(PagingCriteria criteria)
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

		table.NewForm(Platform.Company.SaveOrganizationAsync, new SysOrganization { ParentId = current?.Id, ParentName = current?.Name });
    }

    [Action] public void Edit(SysOrganization row) => table.EditForm(Platform.Company.SaveOrganizationAsync, row);
    [Action] public void Delete(SysOrganization row) => table.Delete(Platform.Company.DeleteOrganizationsAsync, row);
    [Action] public void DeleteM() => table.DeleteM(Platform.Company.DeleteOrganizationsAsync);

    private async void OnNodeClick(MenuItem item)
    {
        current = item;
        await table.RefreshAsync();
    }

    private async Task OnTreeRefresh()
    {
        var datas = await Platform.Company.GetOrganizationsAsync();
        tree.Data = datas.ToMenuItems(ref current);
    }
}