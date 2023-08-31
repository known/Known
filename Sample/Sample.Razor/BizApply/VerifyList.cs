using Sample.Razor.BizApply.Forms;

namespace Sample.Razor.BizApply;

class VerifyList : WebGridView<TbApply, ApplyForm>
{
    protected override Task InitPageAsync()
    {
        Column(c => c.BizType).Template((b, r) => b.Text(r.BizType.ToString()));
        Column(c => c.BizNo).Template((b, r) => b.Link(r.BizNo, Callback(() => View(r))));
        Column(c => c.BizStatus).Template((b, r) => b.StatusTag(r.BizStatus)).Select(new SelectOption(FlowStatus.Verifing));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryData(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Verify;
        return Client.Apply.QueryApplysAsync(criteria);
    }

    public void Pass() => OnPassFlow();
    public void Return() => OnReturnFlow();
    public void Verify(TbApply row) => OnDetail(row, PageType.Verify);

    private void OnPassFlow()
    {
        SelectRows(items =>
        {
            UI.VerifyFlow(Platform.Flow, new FlowFormInfo
            {
                BizId = string.Join(",", items.Select(i => i.Id)),
                BizStatus = FlowStatus.VerifyPass,
                Model = items
            }, Refresh);
        });
    }

    private void OnReturnFlow()
    {
        SelectRows(items =>
        {
            UI.VerifyFlow(Platform.Flow, new FlowFormInfo
            {
                BizId = string.Join(",", items.Select(i => i.Id)),
                BizStatus = FlowStatus.VerifyFail,
                Model = items
            }, Refresh);
        });
    }

    private void OnDetail(TbApply model, PageType type)
    {
        var action = type == PageType.Verify ? "审核" : "查看";
        UI.ShowForm<ApplyForm>($"{action}{Name}", model, action: attr =>
        {
            attr.Set(c => c.PageType, type)
                .Set(c => c.OnSuccess, CloseForm);
        });
    }
}