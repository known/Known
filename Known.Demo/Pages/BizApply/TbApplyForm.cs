using Known.Blazor;
using Known.Demo.Entities;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//申请表单，继承流程表单基类
class TbApplyForm : BaseFlowForm<TbApply>
{
    protected override async Task OnInitFormAsync()
    {
        //添加表单信息Tab
        Tabs.Clear();
        Tabs.Add(new ItemModel("BasicInfo") { Content = BuildBaseInfo });
        Tabs.Add(new ItemModel("TableList") { Content = BuildBillList });
        await base.OnInitFormAsync();
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        builder.Component<FlowForm<TbApply>>()
               .Set(c => c.Model, Model)
               .Set(c => c.Content, b =>
               {
                   b.Div("apply-form", () =>
                   {
                       b.Component<Barcode>().Set(c => c.Id, "bcBizNo").Set(c => c.Value, Model.Data.BizNo).Build();
                       UI.BuildForm(b, Model);
                   });
               })
               .Build();
    }

    private void BuildBillList(RenderTreeBuilder builder)
    {
    }
}