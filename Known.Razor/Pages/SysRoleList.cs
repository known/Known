using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysRoleList : DataGrid<SysRole, SysRoleForm>
{
    protected override Task<PagingResult<SysRole>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.Role.QueryRolesAsync(criteria);
    }

    public void New() => ShowForm(new SysRole { Enabled = true });
    public void DeleteM() => DeleteRows(Platform.Role.DeleteRolesAsync);
    public void Edit(SysRole row) => ShowForm(row);
    public void Delete(SysRole row) => DeleteRow(row, Platform.Role.DeleteRolesAsync);
    public override void View(SysRole row) => UI.ShowForm<SysRoleForm>("查看角色", row);

    protected override void ShowForm(SysRole model = null)
    {
        var action = model == null || model.IsNew ? "新增" : "编辑";
        ShowForm<SysRoleForm>($"{action}角色", model);
    }
}