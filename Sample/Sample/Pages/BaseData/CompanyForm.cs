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
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var json = await System.GetCompanyAsync();
        var data = Utils.FromJson<CompanyInfo>(json);
        Model = new FormModel<CompanyInfo>(this, true) { IsView = true, Data = data };
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return System.SaveCompanyAsync(model);
    }
}