using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.Razor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务申请列表
class BaApplyList : BasePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Form.Width = 800;    //定义表单宽度
        Page.Form.NoFooter = true;//表单不显示默认底部按钮
        Page.Table.Column(c => c.BizStatus).Template(BuildBizStatus);//自定义状态列
    }

    //列表分页查询
    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Apply;
        return Service.QueryApplysAsync(criteria);
    }

    //新增按钮事件
    [Action]
    public async void New()
    {
        var row = await Service.GetDefaultApplyAsync(ApplyType.Test);
        Page.NewForm(Service.SaveApplyAsync, row);
    }

    //编辑操作
    [Action] public void Edit(TbApply row) => Page.EditForm(Service.SaveApplyAsync, row);
    //删除操作
    [Action] public void Delete(TbApply row) => Page.Delete(Service.DeleteApplysAsync, row);
    //批量删除操作
    [Action] public void DeleteM() => Page.DeleteM(Service.DeleteApplysAsync);
    //提交审核
    [Action] public void Submit(TbApply row) { }
    //撤回
    [Action] public void Revoke(TbApply row) { }

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}