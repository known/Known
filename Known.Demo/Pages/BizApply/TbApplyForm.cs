using Known.Demo.Entities;
using Known.Extensions;
using Known.Razor;
using KnownAntDesign.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//申请表单，继承流程表单基类
class TbApplyForm : BaseFlowForm<TbApply>
{
    protected override async Task OnInitFormAsync()
    {
        //添加流程步骤
        Steps.Clear();
        Steps.Add(new ItemModel("业务申请"));
        Steps.Add(new ItemModel("业务审核"));
        Steps.Add(new ItemModel("流程结束"));

        //添加表单信息Tab
        Tabs.Clear();
        Tabs.Add(new ItemModel("基本信息") { Content = BuildBaseInfo });
        await base.OnInitFormAsync();
    }

    private void BuildBaseInfo(RenderTreeBuilder builder)
    {
        builder.Component<FlowForm<TbApply>>()
               .Set(c => c.Model, Model)
               .Set(c => c.Content, b => b.Component<DataForm<TbApply>>().Set(c => c.Model, Model).Build())
               .Build();
    }
}