using Known.Entities;

namespace Known.Razor;

class SysRoleList : BasePage<SysRole>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.FormWidth = 1000;
    }

    protected override Task<PagingResult<SysRole>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.Role.QueryRolesAsync(criteria);
    }

    public void New() => Page.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    public void Edit(SysRole row) => Page.EditForm(Platform.Role.SaveRoleAsync, row);
    public void Delete(SysRole row) => Page.Delete(Platform.Role.DeleteRolesAsync, row);
    public void DeleteM() => Page.DeleteM(Platform.Role.DeleteRolesAsync);
}

public class SysRoleForm : BaseForm<SysRole>
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
    }
}