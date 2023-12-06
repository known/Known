using Known.Blazor;
using Known.Demo.Models;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Demo.Pages.BaseData;

class CompanyForm1 : BasePage
{
    private bool isEdit = false;
    private TabModel modelTab;
    private FormModel<CompanyInfo> formModel;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        modelTab = new TabModel();
        modelTab.Items.Add(new ItemModel("基本信息") { Content = BuildBaseInfo });

        formModel = new FormModel<CompanyInfo>(UI);
        formModel.Data = await Platform.GetCompanyAsync<CompanyInfo>();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildTabs(builder, modelTab);
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        UI.BuildForm(builder, formModel);
        builder.Div("center", () =>
        {
            if (!isEdit)
            {
                UI.Button(builder, "编辑", Callback<MouseEventArgs>(e => Edit()), "primary");
            }
            else
            {
                UI.Button(builder, "保存", Callback<MouseEventArgs>(e => OnSave()), "primary");
                UI.Button(builder, "取消", Callback<MouseEventArgs>(e => OnEdit(false)), "default");
            }
        });
    }

    [Action] public void Edit() => OnEdit(true);

    private void OnEdit(bool edit) => isEdit = edit;

    private async void OnSave()
    {
        var result = await Platform.SaveCompanyAsync(formModel.Data);
        UI.Result(result, () => isEdit = false);
    }
}