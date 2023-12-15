using Known.Blazor;
using Known.Demo.Models;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Demo.Pages.BaseData;

class CompanyForm : BaseTabPage
{
	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
        Tab.Items.Add(new ItemModel("基本信息") { Content = builder => builder.Component<CompanyBaseInfo>().Build() });
    }

	[Action] public void Edit() { }
}

class CompanyBaseInfo : BaseForm<CompanyInfo>
{
    private bool isEdit = false;

	protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Model = new FormModel<CompanyInfo>(UI)
        {
            IsView = true,
            Data = await Platform.GetCompanyAsync<CompanyInfo>()
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-company", () =>
        {
            Model.IsView = !isEdit;
            base.BuildRenderTree(builder);
            if (HasButton("编辑"))
            {
                builder.Div("col-offset-4", () =>
                {
                    if (!isEdit)
                    {
                        UI.Button(builder, "编辑", this.Callback<MouseEventArgs>(e => OnEdit(true)), "primary");
                    }
                    else
                    {
                        UI.Button(builder, "保存", this.Callback<MouseEventArgs>(e => OnSave()), "primary");
                        UI.Button(builder, "取消", this.Callback<MouseEventArgs>(e => OnEdit(false)), "default");
                    }
                });
            }
        });
    }

    private async void OnSave()
    {
        if (!Model.Validate())
            return;

        var result = await Platform.SaveCompanyAsync(Model.Data);
        UI.Result(result, () => OnEdit(false));
    }

    private void OnEdit(bool edit) => isEdit = edit;
}