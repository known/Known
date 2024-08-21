namespace Sample.Client.Pages.BizApply;

//业务审核列表
[Route("/bas/verifies")]
public class BaVerifyList : BaseTablePage<TbApply>
{
    private IApplyService Service;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IApplyService>();

        Table.FormType = typeof(ApplyForm);
        Table.OnQuery = Service.QueryApplysAsync;
        Table.TopStatis = this.BuildTree<PagingResult<TbApply>>((b, r) =>
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<div style=\"padding-left:10px;color:#108ee9;\">");
            sb.Append($"总数：<span style=\"font-weight:bold\">{r?.TotalCount}</span>，");
            sb.Append($"待审核：<span style=\"font-weight:bold\">{r?.Statis?.GetValue<int>("VerifingCount")}</span>");
            sb.Append("</div>");
            b.Markup(sb.ToString());
        });
        Table.Column(c => c.BizStatus).Template((b, r) => b.Tag(r.BizStatus));
    }

	//审核操作
    public void Verify(TbApply row) => this.VerifyFlow(row);
}