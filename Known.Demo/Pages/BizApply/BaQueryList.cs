using Known.Blazor;
using Known.Demo.Entities;
using Known.Demo.Services;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务查询列表
class BaQueryList : BaseTablePage<TbApply>
{
    private ApplyService Service => new() { CurrentUser = CurrentUser };

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Model.OnQuery = QueryApplysAsync;
		Model.Form.Width = 800;
        Model.Column(c => c.BizNo).DefaultDescend();
        Model.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

    //重新申请
    [Action] public void Repeat() => Model.SelectRows(this.RepeatFlow);
    //导出列表
    [Action] public void Export() { }
    //打印
    [Action] public void Print(TbApply row) { }

	private Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria)
	{
		criteria.Parameters[nameof(PageType)] = PageType.Query;
		return Service.QueryApplysAsync(criteria);
	}

	private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BizStatus(builder, row.BizStatus);
}