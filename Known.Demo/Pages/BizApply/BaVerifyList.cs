﻿using Known.Blazor;
using Known.Demo.Entities;
using Known.Demo.Services;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Demo.Pages.BizApply;

//业务审核列表
class BaVerifyList : BaseTablePage<TbApply>
{
    private ApplyService Service => new(Context);

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.FormType = typeof(ApplyForm);
        Table.OnQuery = criteria => Service.QueryApplysAsync(FlowPageType.Verify, criteria);
		Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

	//审核操作
    public void Verify(TbApply row) => this.VerifyFlow(row);

	private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BuildTag(builder, row.BizStatus);
}