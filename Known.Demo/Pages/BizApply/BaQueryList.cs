using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.Razor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

class BaQueryList : BasePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Query;
        return Service.QueryApplysAsync(criteria);
    }

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}