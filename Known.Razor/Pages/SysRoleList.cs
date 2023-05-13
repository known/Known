using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysRoleList : DataGrid<SysRole, SysRoleForm>
{
    public SysRoleList()
    {
        Name = "角色";
    }

    protected override Task<PagingResult<SysRole>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.Role.QueryRolesAsync(criteria);
    }

    public void New() => ShowForm(new SysRole { Enabled = true });
    public void DeleteM() => OnDeleteM(Platform.Role.DeleteRolesAsync);
    public void Edit(SysRole row) => ShowForm(row);
    public void Delete(SysRole row) => OnDelete(row, Platform.Role.DeleteRolesAsync);
}