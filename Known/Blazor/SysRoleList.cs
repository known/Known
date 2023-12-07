using Known.Entities;

namespace Known.Blazor;

class SysRoleList : BaseTablePage<SysRole>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Model.OnQuery = Platform.Role.QueryRolesAsync;
		Model.RowKey = r => r.Id;
        Model.Form.Width = 1000;
    }

    [Action] public void New() => Model.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    [Action] public void Edit(SysRole row) => Model.EditForm(Platform.Role.SaveRoleAsync, row);
    [Action] public void Delete(SysRole row) => Model.Delete(Platform.Role.DeleteRolesAsync, row);
    [Action] public void DeleteM() => Model.DeleteM(Platform.Role.DeleteRolesAsync);
}

public class SysRoleForm : BaseForm<SysRole>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
    }
}