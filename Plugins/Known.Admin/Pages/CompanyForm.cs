namespace Known.Pages;

[Route("/bds/company")]
[Menu(Constants.BaseData, "企业信息", "idcard", 1)]
//[PagePlugin("企业信息", "idcard", PagePluginType.Module, AdminLanguage.BaseData, Sort = 1)]
public class CompanyForm : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        foreach (var item in AdminConfig.CompanyTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}

class CompanyBaseInfo : BaseEditForm<CompanyInfo>
{
    private ICompanyService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ICompanyService>();

        var json = await Service.GetCompanyAsync();
        var data = Utils.FromJson<CompanyInfo>(json) ?? new CompanyInfo();
        Model = new FormModel<CompanyInfo>(this, true) { IsView = true, Data = data };
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return Service.SaveCompanyAsync(model);
    }
}