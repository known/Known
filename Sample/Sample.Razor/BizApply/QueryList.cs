using Sample.Razor.BizApply.Forms;

namespace Sample.Razor.BizApply;

class QueryList : WebGridView<TbApply, ApplyForm>
{
    protected override Task InitPageAsync()
    {
        Column(c => c.BizStatus).Template((b, r) => b.BillStatus(r.BizStatus));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryData(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Query;
        return Client.Apply.QueryApplysAsync(criteria);
    }

    public void Repeat() => OnRepeatFlow();
    public void ExportPage() => Export(ExportMode.Page);
    public void ExportQuery() => Export(ExportMode.Query);
    public void ExportAll() => Export(ExportMode.All);

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