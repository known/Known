namespace Known.Pages;

/// <summary>
/// 租户管理页面组件类。
/// </summary>
[Route("/dev/tenants")]
[DevPlugin("租户管理", "apartment", Sort = 0)]
public class TenantPage : BaseTablePage<SysCompany>
{
    private ICompanyService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ICompanyService>();

        Table.SetAdminTable();
        Table.FormType = typeof(TenantForm);
        Table.Form = new FormInfo { Width = 900 };
        Table.OnQuery = Service.QueryTenantsAsync;
    }

    /// <summary>
    /// 新增租户。
    /// </summary>
    [Action] public void New() => Table.NewForm(Service.SaveTenantAsync, new SysCompany());

    /// <summary>
    /// 编辑租户。
    /// </summary>
    /// <param name="row">租户信息。</param>
    [Action] public void Edit(SysCompany row) => Table.EditForm(Service.SaveTenantAsync, row);

    /// <summary>
    /// 删除租户。
    /// </summary>
    /// <param name="row">租户信息。</param>
    [Action] public void Delete(SysCompany row) => Table.Delete(Service.DeleteTenantsAsync, row);

    /// <summary>
    /// 批量删除租户。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteTenantsAsync);
}