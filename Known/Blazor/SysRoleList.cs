using Known.Entities;

namespace Known.Blazor;

class SysRoleList : BaseTablePage<SysRole>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.Form.Width = 1000;
        Table.OnQuery = Platform.Role.QueryRolesAsync;
		Table.RowKey = r => r.Id;
    }

    [Action] public void New() => Table.NewForm(Platform.Role.SaveRoleAsync, new SysRole());
    [Action] public void Edit(SysRole row) => Table.EditForm(Platform.Role.SaveRoleAsync, row);
    [Action] public void Delete(SysRole row) => Table.Delete(Platform.Role.DeleteRolesAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Platform.Role.DeleteRolesAsync);
}

public class SysRoleForm : BaseForm<SysRole>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Data = await Platform.Role.GetRoleAsync(Model.Data.Id);
    }
}