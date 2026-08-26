namespace Known.Pages;

/// <summary>
/// 企业信息表单页面组件类。
/// </summary>
[Route("/bds/company")]
[Menu(Constants.BaseData, "企业信息", "idcard", 1)]
//[PagePlugin("企业信息", "idcard", PagePluginType.Module, AdminLanguage.BaseData, Sort = 1)]
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
    private ICompanyService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<ICompanyService>();

        Model = new FormModel<CompanyInfo>(this, true) { IsView = true, Data = new() };
    }

    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (!firstRender)
            return;

        var json = await Service.GetCompanyAsync();
        Model.Data = Utils.FromJson<CompanyInfo>(json) ?? new CompanyInfo();
    }

    protected override Task<Result> OnSaveAsync(CompanyInfo model)
    {
        return Service.SaveCompanyAsync(model);
    }
}