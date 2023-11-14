using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Razor;

namespace Known.Demo.Pages.BizApply;

class BaVerifyList : BasePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override Task OnInitPageAsync()
    {
        return base.OnInitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Verify;
        return Service.QueryApplysAsync(criteria);
    }
}