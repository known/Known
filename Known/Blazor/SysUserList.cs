using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysUserList : BasePage<SysUser>
{
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;
	private TreeModel tree;
	private TableModel<SysUser> table;

	protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

		orgs = await Platform.Company.GetOrganizationsAsync();
		var hasOrg = orgs != null && orgs.Count > 1;
		if (hasOrg)
		{
			Page.Type = PageType.Column;
			Page.Spans = "28";

			currentOrg = orgs[0];
			tree = new TreeModel
			{
				ExpandRoot = true,
				Data = orgs.ToMenuItems(),
				OnNodeClick = OnNodeClick,
				SelectedKeys = [currentOrg.Id]
			};

            Page.AddItem("kui-card", BuildTree);
        }

		table = new TableModel<SysUser>(this)
		{
			RowKey = r => r.Id,
			OnQuery = OnQueryUsersAsync,
			OnAction = OnActionClick
		};
        table.Toolbar.OnItemClick = OnToolClick;
		table.Column(c => c.Gender).Template(BuildGender);
        Page.AddItem(BuildTable);
    }

    public override Task RefreshAsync() => table.RefreshAsync();

    private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => builder.BuildTablePage(table);

	private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return Platform.User.QueryUsersAsync(criteria);
    }

    public void New() => table.NewForm(Platform.User.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });
    public void Edit(SysUser row) => table.EditForm(Platform.User.SaveUserAsync, row);
    public void Delete(SysUser row) => table.Delete(Platform.User.DeleteUsersAsync, row);
    public void DeleteM() => table.DeleteM(Platform.User.DeleteUsersAsync);
    public void ResetPassword() => table.SelectRows(Platform.User.SetUserPwdsAsync, Language.Reset);
    public void ChangeDepartment() => table.SelectRows(OnChangeDepartment);
    public void Enable() => table.SelectRows(Platform.User.EnableUsersAsync, Language.Enable);
    public void Disable() => table.SelectRows(Platform.User.DisableUsersAsync, Language.Disable);

    private void BuildGender(RenderTreeBuilder builder, SysUser row) => UI.BuildTag(builder, row.Gender);

    private void OnChangeDepartment(List<SysUser> rows)
    {
        SysOrganization node = null;
        var model = new DialogModel
        {
            Title = Language["Title.ChangeDepartment"],
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandRoot = true,
                    Data = orgs.ToMenuItems(),
                    OnNodeClick = n => node = n.Data as SysOrganization
                });
            }
        };
        model.OnOk = async () =>
        {
            if (node == null)
            {
                UI.Error(Language["Tip.SelectChangeOrganization"]);
                return;
            }

            rows.ForEach(m => m.OrgNo = node.Id);
            var result = await Platform.User.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                //TODO：更换部门后，部门名称未刷新问题
                await model.CloseAsync();
                await table.RefreshAsync();
            });
        };
        UI.ShowDialog(model);
    }

    private async void OnNodeClick(MenuItem item)
    {
        currentOrg = item.Data as SysOrganization;
        await table.RefreshAsync();
    }
}

class SysUserForm : BaseForm<SysUser>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Initialize();
        Model.Data = await Platform.User.GetUserAsync(Model.Data);
        Model.Codes["Roles"] = Model.Data.Roles;
        Model.Field(f => f.UserName).ReadOnly(!Model.Data.IsNew);
        Model.AddRow().AddColumn(c => c.RoleIds);
    }
}