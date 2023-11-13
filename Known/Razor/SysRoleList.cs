using Known.Entities;

namespace Known.Razor;

class SysRoleList : BasePage<SysRole>
{
    protected override Task<PagingResult<SysRole>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.Role.QueryRolesAsync(criteria);
    }

    public void New() => Table.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    public void Edit(SysRole row) => Table.EditForm(Platform.Role.SaveRoleAsync, row);
    public void Delete(SysRole row) => Table.Delete(Platform.Role.DeleteRolesAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.Role.DeleteRolesAsync);
}

public class SysRoleForm : BaseForm<SysRole>
{

}