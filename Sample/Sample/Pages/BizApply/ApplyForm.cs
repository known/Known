namespace Sample.Pages.BizApply;

//申请表单，继承流程表单基类
class ApplyForm : BaseFlowForm<TbApply>
{
    protected override async Task OnInitFormAsync()
    {
        //添加表单信息Tab
        Tab.AddTab("BasicInfo", BuildBaseInfo);
        Tab.AddTab("TableList", BuildBillList);
        await base.OnInitFormAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            //Thread.Sleep(1000);
        }
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        builder.Component<FlowForm<TbApply>>()
               .Set(c => c.Model, Model)
               .Set(c => c.Content, b =>
               {
                   b.Div("apply-form", () =>
                   {
                       b.Component<KBarcode>().Set(c => c.Value, Model.Data?.BizNo).Build();
                       b.Form(Model);
                   });
               })
               .Build();
    }

    private void BuildBillList(RenderTreeBuilder builder)
    {
        builder.Component<ApplyListTable>()
               .Set(c => c.ReadOnly, Model.IsView)
               .Set(c => c.Head, Model.Data)
               .Build();
    }
}