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

		//页面类型，左右布局
		Page.Type = PageType.Column;
		Page.Spans = [4, 20];
		Page.Contents = [BuildTree, BuildTable];

		//左侧组织架构树模型
		tree = new TreeModel
		{
			ExpandRoot = true,
			OnNodeClick = OnNodeClick,
			OnModelChanged = OnTreeModelChanged
        };
        tree.Load();

        //右侧组织架构表格模型
        table = new TablePageModel<SysOrganization>(this)
		{
			FormTitle = row => $"{Name} - {row.ParentName}",
			RowKey = r => r.Id,
			ShowPager = false,
			OnQuery = OnQueryOrganizationsAsync,
			OnAction = OnActionClick
		};
        table.Toolbar.OnItemClick = OnToolClick;
	}

	public override async Task RefreshAsync()
	{
		await tree.RefreshAsync();
		await table.RefreshAsync();
	}

	private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => builder.BuildTablePage(table);

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

    private async void OnTreeModelChanged(TreeModel model)
    {
        var datas = await Platform.Company.GetOrganizationsAsync();
        if (datas != null && datas.Count > 0)
        {
            tree.Data = datas.ToMenuItems(ref current);
            tree.SelectedKeys = [current?.Id];
        }
        model.Data = tree.Data;
        model.SelectedKeys = tree.SelectedKeys;
    }
}