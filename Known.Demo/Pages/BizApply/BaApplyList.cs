using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.Razor;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

class BaApplyList : BasePage<TbApply>
{
    private readonly string BizStatus = $"{FlowStatus.Save},{FlowStatus.Verifing},{FlowStatus.VerifyFail},{FlowStatus.ReApply}";
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Form.Width = 800;
        Page.Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

    protected override Task<PagingResult<TbApply>> OnQueryAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Apply;
        return Service.QueryApplysAsync(criteria);
    }

    [Action]
    public async void New()
    {
        var row = await Service.GetDefaultApplyAsync(ApplyType.Test);
        Page.NewForm(Service.SaveApplyAsync, row);
    }

    [Action] public void Edit(TbApply row) => Page.EditForm(Service.SaveApplyAsync, row);
    [Action] public void Delete(TbApply row) => Page.Delete(Service.DeleteApplysAsync, row);
    [Action] public void DeleteM() => Page.DeleteM(Service.DeleteApplysAsync);

    private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}