namespace Known.Demo.Pages.BizApply;

//申请表单，继承流程表单基类
class ApplyForm : BaseFlowForm<TbApply>
{
    protected override async Task OnInitPageAsync()
    {
        //添加表单信息Tab
        Tab.AddTab("BasicInfo", BuildBaseInfo);
        Tab.AddTab("TableList", BuildBillList);
        await base.OnInitPageAsync();
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        builder.Component<FlowForm<TbApply>>()
               .Set(c => c.Model, Model)
               .Set(c => c.Content, b =>
               {
                   b.Div("apply-form", () =>
                   {
                       b.Component<KBarcode>().Set(c => c.Id, "bcBizNo").Set(c => c.Value, Model.Data.BizNo).Build();
                       UI.BuildForm(b, Model);
                   });
               })
               .Build();
    }

    private void BuildBillList(RenderTreeBuilder builder)
    {
    }
}