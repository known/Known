namespace Known.Demo.Pages.BaseData;

[Route("/bds/company")]
public class CompanyForm : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.AddTab("BasicInfo", b => b.Component<CompanyBaseInfo>().Build());
    }
}

class CompanyBaseInfo : BaseEditForm<CompanyInfo>
{
    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<CompanyInfo>(Context)
        {
            IsView = true,
            Data = await Platform.GetCompanyAsync<CompanyInfo>()
        };
        await base.OnInitFormAsync();
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return Platform.SaveCompanyAsync(model);
    }
}