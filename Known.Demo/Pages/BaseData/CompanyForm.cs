using Known.Blazor;
using Known.Demo.Models;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Demo.Pages.BaseData;

class CompanyForm : BasePage
{
    private TabModel model;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        model = new TabModel();
        model.Items.Add(new ItemModel("基本信息") { Content = builder => builder.Component<CompanyBaseInfo>().Build() });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, model);

    [Action] public void Edit() { }
}

class CompanyBaseInfo : BaseComponent
{
    private bool isEdit = false;
    private FormModel<CompanyInfo> model;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        model = new FormModel<CompanyInfo>(UI)
        {
            IsView = true,
            Data = await Platform.GetCompanyAsync<CompanyInfo>()
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-company", () =>
        {
            model.IsView = !isEdit;
            UI.BuildForm(builder, model);
            builder.Div("center", () =>
            {
                if (!isEdit)
                {
                    UI.Button(builder, "编辑", Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                }
                else
                {
                    UI.Button(builder, "保存", Callback<MouseEventArgs>(e => OnSave()), "primary");
                    UI.Button(builder, "取消", Callback<MouseEventArgs>(e => OnEdit(false)), "default");
                }
            });
        });
    }

    private async void OnSave()
    {
        var result = await Platform.SaveCompanyAsync(model.Data);
        UI.Result(result, () => OnEdit(false));
    }

    private void OnEdit(bool edit) => isEdit = edit;
}