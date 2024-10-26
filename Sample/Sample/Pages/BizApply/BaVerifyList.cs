using AntDesign;

namespace Sample.Pages.BizApply;

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
        Table.Column(c => c.BizNo).Template((b, r) => b.Link(r.BizNo, this.Callback(() =>
        {
            Navigation.NavigateTo($"/bas/verfiy/{r.BizNo}");
        })));
        Table.Column(c => c.BizStatus).Template((b, r) => b.Tag(r.BizStatus));
    }

	//审核操作
    public void Verify(TbApply row) => this.VerifyFlow(row);
}

[Route("/bas/verfiy/{id}")]
[DisplayName("审核详情")]
public class BaVerifyDetail : BaseComponent, IReuseTabsPage
{
    public RenderFragment GetPageTitle()
    {
        return this.BuildTree(b =>
        {
            b.Icon("file");
            b.Span($"审核详情-{Id}");
        });
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var model = new FormModel<TbApply>(this);
        model.Data = new TbApply();
        builder.Component<ApplyForm>()
               .Set(c=>c.Model, model)
               .Build();
    }
}