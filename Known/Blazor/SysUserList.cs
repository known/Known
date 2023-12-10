using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysUserList : BasePage<SysUser>
{
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;
	private TreeModel tree;
	private TablePageModel<SysUser> table;

	protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

		orgs = await Platform.Company.GetOrganizationsAsync();
		var hasOrg = orgs != null && orgs.Count > 1;
		if (hasOrg)
		{
			Page.Type = PageType.Column;
			Page.Spans = [4, 20];

			currentOrg = orgs[0];
			tree = new TreeModel
			{
				ExpandRoot = true,
				Data = orgs.ToMenuItems(),
				OnNodeClick = OnNodeClick,
				SelectedKeys = [currentOrg.Id]
			};

			Page.Contents.Add(BuildTree);
		}

		table = new TablePageModel<SysUser>(this)
		{
			RowKey = r => r.Id,
			OnQuery = OnQueryUsersAsync,
			OnToolClick = OnToolClick,
			OnAction = OnActionClick
		};
		table.Form.Width = 800;
		table.Column(c => c.Department).Visible(hasOrg);
		table.Column(c => c.Gender).Template(BuildGender);
		Page.Contents.Add(BuildTable);
    }

	public override async Task RefreshAsync()
	{
        //TODO：保存时表格刷新问题
		//await tree.RefreshAsync();
		//model.StateChanged.Invoke();
		await table.RefreshAsync();
		//StateChanged();
	}

	private void BuildTree(RenderTreeBuilder builder) => builder.Div("p10", () => UI.BuildTree(builder, tree));
	private void BuildTable(RenderTreeBuilder builder) => UI.BuildPage(builder, table);

	private Task<PagingResult<SysUser>> OnQueryUsersAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return Platform.User.QueryUsersAsync(criteria);
    }

    [Action] public void New() => table.NewForm(Platform.User.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });
    [Action] public void Edit(SysUser row) => table.EditForm(Platform.User.SaveUserAsync, row);
    [Action] public void Delete(SysUser row) => table.Delete(Platform.User.DeleteUsersAsync, row);
    [Action] public void DeleteM() => table.DeleteM(Platform.User.DeleteUsersAsync);
    [Action] public void ResetPassword() => table.SelectRows(Platform.User.SetUserPwdsAsync, "重置");
    [Action] public void ChangeDepartment() => table.SelectRows(OnChangeDepartment);
    [Action] public void Enable() => table.SelectRows(Platform.User.EnableUsersAsync, "启用");
    [Action] public void Disable() => table.SelectRows(Platform.User.DisableUsersAsync, "禁用");

    private void BuildGender(RenderTreeBuilder builder, SysUser row)
    {
        var color = row.Gender == "男" ? "#108ee9" : "hotpink";
        UI.BuildTag(builder, row.Gender, color);
    }

    private void OnChangeDepartment(List<SysUser> rows)
    {
        SysOrganization node = null;
        var model = new ModalOption
        {
            Title = "更换部门",
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
                UI.Error("请选择更换的部门！");
                return;
            }

            rows.ForEach(m => m.OrgNo = node.Id);
            var result = await Platform.User.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                await model.OnClose?.Invoke();
                await table.RefreshAsync();
            });
        };
        UI.ShowModal(model);
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
        Model.Data = await Platform.User.GetUserAsync(Model.Data);
        Model.Column(c => c.UserName).ReadOnly(!Model.Data.IsNew);
        Model.Codes["Roles"] = Model.Data.Roles;
    }
}