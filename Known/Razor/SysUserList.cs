using Known.Entities;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class SysUserList : BasePage<SysUser>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.FormWidth = 800;
        Page.Table.Column(c => c.Gender).Template(BuildGender);
    }

    protected override Task<PagingResult<SysUser>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.User.QueryUsersAsync(criteria);
    }

    public void New() => Page.NewForm(Platform.User.SaveUserAsync, new SysUser());
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
        //KTreeItem<SysOrganization> node = null;
        //UI.Prompt("更换部门", new(300, 300), builder =>
        //{
        //    builder.Component<KTree<SysOrganization>>()
        //           .Set(c => c.Data, data)
        //           .Set(c => c.OnItemClick, Callback<KTreeItem<SysOrganization>>(n => node = n))
        //           .Build();
        //}, async model =>
        //{
        //    rows.ForEach(m => m.OrgNo = node.Value.Id);
        //    var result = await Platform.User.ChangeDepartmentAsync(rows);
        //    UI.Result(result, () =>
        //    {
        //        Refresh();
        //        UI.CloseDialog();
        //    });
        //});
    }
}

public class SysUserForm : BaseForm<SysUser>
{

}