namespace Known.Pages;

/// <summary>
/// 企业信息页面组件类。
/// </summary>
[Route("/bds/company")]
[Menu(Constants.BaseData, "企业信息", "idcard", 1)]
public class CompanyForm : BaseTabPage
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        foreach (var item in UIConfig.CompanyTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}

class CompanyBaseInfo : BaseEditForm<CompanyInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var json = await Admin.GetCompanyAsync();
        var data = Utils.FromJson<CompanyInfo>(json) ?? new CompanyInfo();
        Model = new FormModel<CompanyInfo>(this, true) { IsView = true, Data = data };
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return Admin.SaveCompanyAsync(model);
    }
}