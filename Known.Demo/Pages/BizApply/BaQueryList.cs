using Known.Blazor;
using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务查询列表
class BaQueryList : BasePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Form.Width = 800;
        Page.Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Query;
        return Service.QueryApplysAsync(criteria);
    }

    //重新申请
    [Action] public void Repeat() { }
    //导出列表
    [Action] public void Export() { }
    //打印
    [Action] public void Print(TbApply row) { }

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}