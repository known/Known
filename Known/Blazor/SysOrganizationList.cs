using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private MenuItem current;
    private PageModel<SysOrganization> model;
    private TreeModel tree;
    private TablePageModel<SysOrganization> table;

    public SysOrganizationList()
    {
        model = new PageModel<SysOrganization>(this);
        model.Type = PageType.Column;
        model.Spans = [4, 20];
        model.Contents = [BuildTree, BuildTable];

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
			ShowPager = false
		};
	}

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();

		//Page.FormTitle = row => $"{Name} - {row.ParentName}";
		//Page.Table.RowKey = r => r.Id;
		//Page.Table.ShowPager = false;

		var datas = await Platform.Company.GetOrganizationsAsync();
		if (datas != null && datas.Count > 0)
		{
			tree.Data = datas.ToMenuItems(ref current);
			current = tree.Data[0];
			tree.SelectedKeys = [current.Id];
		}
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		UI.BuildPage(builder, model);
	}

	private void BuildTree(RenderTreeBuilder builder) => UI.BuildTree(builder, tree);
	private void BuildTable(RenderTreeBuilder builder) => UI.BuildTablePage(builder, table);

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
        tree.Data = datas.ToMenuItems(ref current);
    }
}