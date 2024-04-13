using Known.Blazor;
using Known.Demo.Models;
using Known.Extensions;

namespace Known.Demo.Pages.BaseData;

class CompanyForm : BaseTabPage
{
    private CompanyBaseInfo info;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.AddTab("BasicInfo", b => b.Component<CompanyBaseInfo>().Build(value => info = value));
    }

    public override void StateChanged()
    {
        info?.StateChanged();
        base.StateChanged();
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