using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysUserList : BasePage<SysUser>
{
    private List<SysOrganization> orgs;
    private SysOrganization currentOrg;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        orgs = await Platform.Company.GetOrganizationsAsync();
        var hasOrg = orgs != null && orgs.Count > 1;
        if (hasOrg)
        {
            currentOrg = orgs[0];
            Page.Tree = new TreeModel
            {
                ExpandParent = true,
                Data = orgs.ToMenuItems(),
                OnNodeClick = OnNodeClick,
                SelectedKeys = [currentOrg.Id]
            };
        }

        Page.Form.Width = 800;
        Page.Table.Column(c => c.Department).Visible(hasOrg);
        Page.Table.Column(c => c.Gender).Template(BuildGender);
    }

    protected override Task<PagingResult<SysUser>> OnQueryAsync(PagingCriteria criteria)
    {
        if (currentOrg != null)
            criteria.Parameters[nameof(SysUser.OrgNo)] = currentOrg?.Id;
        return Platform.User.QueryUsersAsync(criteria);
    }

    public void New() => Page.NewForm(Platform.User.SaveUserAsync, new SysUser { OrgNo = currentOrg?.Id });
    public void Edit(SysUser row) => Page.EditForm(Platform.User.SaveUserAsync, row);
    public void Delete(SysUser row) => Page.Delete(Platform.User.DeleteUsersAsync, row);
    public void DeleteM() => Page.DeleteM(Platform.User.DeleteUsersAsync);
    public void ResetPassword() => Page.Table.SelectRows(Platform.User.SetUserPwdsAsync, "重置");
    public void ChangeDepartment() => Page.Table.SelectRows(OnChangeDepartment);
    public void Enable() => Page.Table.SelectRows(Platform.User.EnableUsersAsync, "启用");
    public void Disable() => Page.Table.SelectRows(Platform.User.DisableUsersAsync, "禁用");

    private void BuildGender(RenderTreeBuilder builder, SysUser row)
    {
        var color = row.Gender == "男" ? "#108ee9" : "hotpink";
        UI.BuildTag(builder, row.Gender, color);
    }

    private void OnChangeDepartment(List<SysUser> rows)
    {
        SysOrganization org = null;
        var model = new ModalOption
        {
            Title = "更换部门",
            Content = builder =>
            {
                UI.BuildTree(builder, new TreeModel
                {
                    ExpandParent = true,
                    Data = orgs.ToMenuItems(),
                    OnNodeClick = n => org = n.Data as SysOrganization
                });
            }
        };
        model.OnOk = async () =>
        {
            if (org == null)
            {
                UI.Error("请选择更换的部门！");
                return;
            }

            rows.ForEach(m => m.OrgNo = org.Id);
            var result = await Platform.User.ChangeDepartmentAsync(rows);
            UI.Result(result, async () =>
            {
                await model.OnClose?.Invoke();
                await Page.Table.RefreshAsync();
            });
        };
		UI.ShowModal(model);
	}

    private async void OnNodeClick(MenuItem item)
    {
        currentOrg = item.Data as SysOrganization;
        await Page.Table.RefreshAsync();
    }
}

public class SysUserForm : BaseForm<SysUser>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Data = await Platform.User.GetUserAsync(Model.Data);
    }
}