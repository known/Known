namespace Known.Demo.Pages.BaseData;

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
        Model = new FormModel<CompanyInfo>(Context, true)
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