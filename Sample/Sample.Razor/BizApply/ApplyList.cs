using Sample.Razor.BizApply.Forms;

namespace Sample.Razor.BizApply;

//申请列表
class ApplyList : WebGridView<TbApply, ApplyForm>
{
    protected override Task InitPageAsync()
    {
        Column(c => c.BizType).Template((b, r) => b.Text(r.BizType.ToString()));
        Column(c => c.BizNo).Template((b, r) => b.Link(r.BizNo, Callback(() => View(r))));
        Column(c => c.BizStatus).Template((b, r) => b.BillStatus(r.BizStatus));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<TbApply>> OnQueryData(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageType)] = PageType.Apply;
        return Client.Apply.QueryApplysAsync(criteria);
    }

    public override bool CheckAction(ButtonInfo action, TbApply item)
    {
        if (item.BizStatus == FlowStatus.Verifing && !action.Is(GridAction.View))
            return false;

        return base.CheckAction(action, item);
    }

    protected override async void ShowForm(TbApply? model = null)
    {
        var action = model == null ? "新增" : "编辑";
        model ??= await Client.Apply.GetDefaultApplyAsync(ApplyType.Test);
        ShowForm<ApplyForm>($"{action}{Name}", model, action: attr =>
        {
            attr.Set(c => c.PageType, PageType.Apply);
        });
    }

    public void New() => ShowForm();
    public void DeleteM() => DeleteRows(Client.Apply.DeleteApplysAsync);
    public void Edit(TbApply row) => ShowForm(row);
    public void Delete(TbApply row) => DeleteRow(row, Client.Apply.DeleteApplysAsync);
    public void Submit(TbApply row) => OnSubmitFlow(row);
    public void Revoke(TbApply row) => OnRevokeFlow(row);

    private void OnSubmitFlow(TbApply row)
    {
        if (!row.CanSubmit)
        {
            UI.Toast($"{row.BizStatus}记录不能提交审核！");
            return;
        }

        var vr = row.ValidCommit();
        if (!vr.IsValid)
        {
            UI.Alert(vr.Message);
            return;
        }

        UI.SubmitFlow(Platform.Flow, new FlowFormInfo
        {
            UserRole = UserRole.Verifier,
            BizId = row.Id,
            BizStatus = FlowStatus.Verifing,
            Model = row
        }, Refresh);
    }

    private void OnRevokeFlow(TbApply row)
    {
        if (!row.CanRevoke)
        {
            UI.Toast("请选择待审核的记录！");
            return;
        }

        UI.RevokeFlow(Platform.Flow, new FlowFormInfo
        {
            BizId = row.Id,
            BizStatus = FlowStatus.Revoked,
            Model = row
        }, Refresh);
    }
}