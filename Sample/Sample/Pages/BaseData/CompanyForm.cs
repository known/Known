namespace Sample.Pages.BaseData;

[Route("/bds/company")]
public class CompanyForm : BaseTabPage
{
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Tab.AddTab("BasicInfo", b => b.Component<CompanyBaseInfo>().Build());
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
        var data = Utils.FromJson<CompanyInfo>(json);
        Model = new FormModel<CompanyInfo>(this, true) { IsView = true, Data = data };
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return Service.SaveCompanyAsync(model);
    }
}