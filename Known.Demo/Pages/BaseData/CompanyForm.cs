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
        model.Items.Add(new ItemModel("基本信息") { Content = BuildBaseInfo });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildTabs(builder, model);
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        builder.Component<CompanyInfoForm>().Build();
    }

    [Action] public void Edit() { }
}

class CompanyInfoForm : BaseComponent
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
            //TODO：使用动态组件刷新表单状态
            if (isEdit)
            {
                model.IsView = false;
                UI.BuildForm(builder, model);
            }
            else
            {
                model.IsView = true;
                UI.BuildForm(builder, model);
            }
            builder.Div("buttons", () =>
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