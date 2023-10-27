using Known.WorkFlows;

namespace Known.Demo.Pages.BizApply;

class QueryList : WebGridView<TbApply, ApplyForm>
{
    protected override Task InitPageAsync()
    {
        Column(c => c.BizType).Template((b, r) => b.Text(r.BizType.ToString()));
        Column(c => c.BizNo).Template((b, r) => b.Link(r.BizNo, Callback(() => View(r))));
        Column(c => c.BizStatus).Template((b, r) => b.StatusTag(r.BizStatus)).Select(new SelectOption(FlowStatus.VerifyPass));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryDataAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Query;
        return Client.Apply.QueryApplysAsync(criteria);
    }

    public void Repeat() => OnRepeatFlow();
    public void ExportPage() => ExportByModeAsync(ExportMode.Page);
    public void ExportQuery() => ExportByModeAsync(ExportMode.Query);
    public void ExportAll() => ExportByModeAsync(ExportMode.All);

    private void OnRepeatFlow()
    {
        SelectRows(items =>
        {
            UI.RepeatFlow(Platform, new FlowFormInfo
            {
                BizId = string.Join(",", items.Select(i => i.Id)),
                BizStatus = FlowStatus.ReApply,
                Model = items
            }, Refresh);
        });
    }
}