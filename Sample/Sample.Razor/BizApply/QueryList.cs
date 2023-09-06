﻿using Sample.Razor.BizApply.Forms;

namespace Sample.Razor.BizApply;

class QueryList : WebGridView<TbApply, ApplyForm>
{
    protected override Task InitPageAsync()
    {
        Column(c => c.BizType).Template((b, r) => b.Text(r.BizType.ToString()));
        Column(c => c.BizNo).Template((b, r) => b.Link(r.BizNo, Callback(() => View(r))));
        Column(c => c.BizStatus).Template((b, r) => b.StatusTag(r.BizStatus)).Select(new SelectOption(FlowStatus.VerifyPass));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryData(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Query;
        return Client.Apply.QueryApplysAsync(criteria);
    }

    public void Repeat() => OnRepeatFlow();
    public void ExportPage() => ExportData(Name, ExportMode.Page);
    public void ExportQuery() => ExportData(Name, ExportMode.Query);
    public void ExportAll() => ExportData(Name, ExportMode.All);

    private void OnRepeatFlow()
    {
        SelectRows(items =>
        {
            UI.RepeatFlow(Platform.Flow, new FlowFormInfo
            {
                BizId = string.Join(",", items.Select(i => i.Id)),
                BizStatus = FlowStatus.ReApply,
                Model = items
            }, Refresh);
        });
    }
}