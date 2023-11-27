using Known.Blazor;
using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务审核列表
class BaVerifyList : BasePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Form.Width = 800;
        Page.Table.Column(c => c.BizNo).DefaultDescend();
        Page.Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Verify;
        return Service.QueryApplysAsync(criteria);
    }

    //审核操作
    [Action] public void Verify(TbApply row) => Page.VerifyFlow(row);

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}