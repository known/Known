using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysOrganizationList : BasePage<SysOrganization>
{
    private MenuItem current;
    private TreeModel tree;
    private TableModel<SysOrganization> table;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();

		Page.Type = PageType.Column;
		Page.Spans = "28";
        Page.AddItem("kui-card", BuildTree);
        Page.AddItem(BuildTable);

        tree = new TreeModel
		{
			ExpandRoot = true,
			OnNodeClick = OnNodeClick,
			OnModelChanged = OnTreeModelChanged
        };
        tree.Load();

        table = new TableModel<SysOrganization>(this)
		{
			FormTitle = row => $"{PageName} - {row.ParentName}",
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
            UI.Error(Language["Tip.SelectParentOrganization"]);
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