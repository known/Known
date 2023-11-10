using Known.Entities;

namespace Known.Razor;

class SysUserList : BasePage<SysUser>
{
    protected override Task<PagingResult<SysUser>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.User.QueryUsersAsync(criteria);
    }

    public void New() => Table.ShowForm(Platform.User.SaveUserAsync, new SysUser());
    public void Edit(SysUser row) => Table.ShowForm(Platform.User.SaveUserAsync, row);
    public void Delete(SysUser row) => Table.Delete(Platform.User.DeleteUsersAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.User.DeleteUsersAsync);
    public void ResetPassword() => Table.SelectRows(Platform.User.SetUserPwdsAsync, "重置");
    public void ChangeDepartment() => Table.SelectRows(OnChangeDepartment);
    public void Enable() => Table.SelectRows(Platform.User.EnableUsersAsync, "启用");
    public void Disable() => Table.SelectRows(Platform.User.DisableUsersAsync, "禁用");

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