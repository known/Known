namespace Known.Demo.Pages.BizApply;

//业务审核列表
[Route("/bas/verifies")]
public class BaVerifyList : BaseTablePage<TbApply>
{
    private ApplyService Service => new(Context);

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table.FormType = typeof(ApplyForm);
        Table.OnQuery = criteria => Service.QueryApplysAsync(FlowPageType.Verify, criteria);
		Table.Column(c => c.BizStatus).Template(BuildBizStatus);
    }

	//审核操作
    public void Verify(TbApply row) => this.VerifyFlow(row);

	private void BuildBizStatus(RenderTreeBuilder builder, TbApply row) => UI.BuildTag(builder, row.BizStatus);
}