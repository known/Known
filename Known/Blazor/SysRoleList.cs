using Known.Entities;

namespace Known.Blazor;

class SysRoleList : BasePage<SysRole>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Form.Width = 1000;
        Page.Table.RowKey = r => r.Id;
    }

    protected override Task<PagingResult<SysRole>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.Role.QueryRolesAsync(criteria);
    }

    [Action] public void New() => Page.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    [Action] public void Edit(SysRole row) => Page.EditForm(Platform.Role.SaveRoleAsync, row);
    [Action] public void Delete(SysRole row) => Page.Delete(Platform.Role.DeleteRolesAsync, row);
    [Action] public void DeleteM() => Page.DeleteM(Platform.Role.DeleteRolesAsync);
}

public class SysRoleForm : BaseForm<SysRole>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
    }
}