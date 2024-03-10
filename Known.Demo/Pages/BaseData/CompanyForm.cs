using Known.Blazor;
using Known.Demo.Models;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

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

class CompanyBaseInfo : BaseForm<CompanyInfo>
{
    private bool isEdit = false;

    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<CompanyInfo>(Context)
        {
            IsView = true,
            Data = await Platform.GetCompanyAsync<CompanyInfo>()
        };
        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("form-company", () =>
        {
            Model.IsView = !isEdit;
            base.BuildForm(builder);
            if (HasButton("Edit"))
            {
                builder.Div("kui-form-page-button", () =>
                {
                    if (!isEdit)
                    {
                        UI.Button(builder, Language.Edit, this.Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                    }
                    else
                    {
                        UI.Button(builder, Language.Save, this.Callback<MouseEventArgs>(OnSaveAsync), "primary");
                        UI.Button(builder, Language.Cancel, this.Callback<MouseEventArgs>(e => OnEdit(false)), "default");
                    }
                });
            }
        });
    }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await Platform.SaveCompanyAsync(Model.Data);
        UI.Result(result, () => OnEdit(false));
    }

    private void OnEdit(bool edit) => isEdit = edit;
}