namespace Known.Develops;

/// <summary>
/// 我的租户页面组件类。
/// </summary>
[Route("/mytenants")]
[DisplayName("我的租户")]
public class TenantMyPage : BaseTablePage<SysCompany>
{
    private ICompanyService Service;

    /// <inheritdoc />
    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("apartment", Language.MyTenants);
    }

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ICompanyService>();

        Table.Name = Language.MyTenants;
        Table.FormType = typeof(TenantForm);
        Table.Form = new FormInfo { Width = 1000 };
        Table.OnQuery = QueryTenantsAsync;
    }

    /// <summary>
    /// 新增租户。
    /// </summary>
    [Action]
    public void New()
    {
        var row = new SysCompany { Manager = CurrentUser.UserName, IsManage = true };
        Table.NewForm(Service.SaveTenantAsync, row);
    }

    /// <summary>
    /// 编辑租户。
    /// </summary>
    /// <param name="row">租户信息。</param>
    [Action]
    public void Edit(SysCompany row) => Table.EditForm(Service.SaveTenantAsync, row);

    /// <summary>
    /// 删除租户。
    /// </summary>
    /// <param name="row">租户信息。</param>
    [Action] public void Delete(SysCompany row) => Table.Delete(Service.DeleteTenantsAsync, row);

    /// <summary>
    /// 批量删除租户。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteTenantsAsync);

    private async Task<PagingResult<SysCompany>> QueryTenantsAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysCompany.Manager), QueryType.Equal, CurrentUser.UserName);
        var result = await Service.QueryTenantsAsync(criteria);
        foreach (var item in result.PageData)
        {
            item.IsManage = true;
        }
        return result;
    }
}